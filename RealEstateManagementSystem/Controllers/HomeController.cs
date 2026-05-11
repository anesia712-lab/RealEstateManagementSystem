using Microsoft.AspNetCore.Mvc;
using RealEstateManagementSystem.Models;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using RealEstateManagementSystem.Infrastructure;

namespace RealEstateManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(
            IConfiguration configuration,
            ILogger<HomeController> logger,
            IWebHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var userName = email?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both email and password.";
                return View();
            }

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                _logger.LogError("DefaultConnection connection string is missing.");
                ViewBag.Error = "The application isn't configured correctly. Please contact support.";
                return View();
            }

            try
            {
                using var conn = new SqlConnection(connStr);
                conn.Open();

                using var cmd = new SqlCommand("SELECT PasswordHash, FullName FROM Users WHERE UserName = @UserName", conn);
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value = userName;

                using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (!reader.Read())
                {
                    ViewBag.Error = "Invalid email or password.";
                    return View();
                }

                var storedHash = reader["PasswordHash"] as string ?? string.Empty;
                var fullName = reader["FullName"] as string ?? string.Empty;
                var role = "admin";

                bool verified = false;
                try
                {
                    verified = BCrypt.Net.BCrypt.Verify(password ?? string.Empty, storedHash);
                }
                catch
                {
                    verified = false;
                }

                if (!verified && !string.IsNullOrEmpty(storedHash) && storedHash == (password ?? string.Empty))
                {
                    verified = true;
                    try
                    {
                        var newHash = BCrypt.Net.BCrypt.HashPassword(password ?? string.Empty);
                        using var updateCmd = new SqlCommand(
                            "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserName = @UserName", conn);
                        updateCmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = newHash;
                        updateCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value = userName;
                        updateCmd.ExecuteNonQuery();
                    }
                    catch (SqlException sex)
                    {
                        _logger.LogError(sex, "Failed to migrate plaintext password for user {UserName}", userName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error migrating password for user {UserName}", userName);
                    }
                }

                if (verified)
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("UserEmail", userName);
                    HttpContext.Session.SetString("UserFullName", fullName);
                    HttpContext.Session.SetString("UserRole", role);
                    return RedirectToAction("Index");
                }

                ViewBag.Error = "Invalid email or password.";
                return View();
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error during login for user {UserName}", userName);
                ViewBag.Error = FriendlyErrors.DescribeSql(sqlEx);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user {UserName}", userName);
                ViewBag.Error = FriendlyErrors.SaveFailedGeneric;
                return View();
            }
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fullName))
            {
                ViewBag.Error = "Please fill in your full name, email, and password.";
                return View();
            }

            if (password.Length < 4)
            {
                ViewBag.Error = "Choose a stronger password (at least 4 characters).";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords don't match.";
                return View();
            }

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                _logger.LogError("DefaultConnection connection string is missing.");
                ViewBag.Error = "The application isn't configured correctly. Please contact support.";
                return View();
            }

            try
            {
                using var conn = new SqlConnection(connStr);
                conn.Open();

                using (var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE UserName = @UserName", conn))
                {
                    checkCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value = email.Trim();
                    var exists = Convert.ToInt32(checkCmd.ExecuteScalar() ?? 0) > 0;
                    if (exists)
                    {
                        ViewBag.Error = "An account with that email already exists. Try signing in instead.";
                        return View();
                    }
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                var role = "admin";
                var createdAt = DateTime.UtcNow;

                using (var insertCmd = new SqlCommand(
                           "INSERT INTO Users (UserName, PasswordHash, FullName, CreatedAt) VALUES (@UserName, @PasswordHash, @FullName, @CreatedAt)",
                           conn))
                {
                    insertCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value = email.Trim();
                    insertCmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = passwordHash;
                    insertCmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 200).Value = fullName.Trim();
                    insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value = createdAt;
                    insertCmd.ExecuteNonQuery();
                }

                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserEmail", email.Trim());
                HttpContext.Session.SetString("UserFullName", fullName.Trim());
                HttpContext.Session.SetString("UserRole", role);

                return RedirectToAction("Index");
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error during sign-up");
                ViewBag.Error = FriendlyErrors.DescribeSql(sqlEx);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during sign-up");
                ViewBag.Error = FriendlyErrors.Describe(ex);
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>Friendly status pages (404, etc.).</summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult HandleStatus(int code = 404)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var model = new ErrorViewModel { RequestId = requestId, StatusCode = code };

            if (code == 404)
            {
                Response.StatusCode = 404;
                ViewBag.PageTitle = "Page not found";
                model.UserMessage = "We couldn't find that page. The link may be outdated or typed incorrectly.";
            }
            else if (code == 403)
            {
                Response.StatusCode = 403;
                ViewBag.PageTitle = "Access denied";
                model.UserMessage = "You're not allowed to view this resource. Sign in or use a different account.";
            }
            else
            {
                Response.StatusCode = code >= 400 && code < 600 ? code : 400;
                ViewBag.PageTitle = "Request issue";
                model.UserMessage = "That request couldn't be completed. Try again or go back to the home page.";
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var model = new ErrorViewModel
            {
                RequestId = requestId,
                UserMessage = "Something went wrong while we were processing your request. Please try again in a moment."
            };

            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (feature?.Error != null)
            {
                _logger.LogError(feature.Error, "Unhandled exception; RequestId {RequestId}", requestId);
                if (_env.IsDevelopment())
                {
                    model.TechnicalDetail = feature.Error.ToString();
                }
            }

            return View(model);
        }
    }
}

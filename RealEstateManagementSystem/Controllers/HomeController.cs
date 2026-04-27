using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using RealEstateManagementSystem.Models;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace RealEstateManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
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
            // Use the posted 'email' field as the user name value
            var userName = email?.Trim() ?? string.Empty;

            // Retrieve connection string from configuration
            var connStr = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using var conn = new SqlConnection(connStr);
                conn.Open();

                // Read stored password hash and full name from Users table
                // Use UserName column (some schemas don't include a Role column)
                using var cmd = new SqlCommand("SELECT PasswordHash, FullName FROM Users WHERE UserName = @UserName", conn);
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value = userName;

                using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (!reader.Read())
                {
                    ViewBag.Error = "Invalid email or password";
                    return View();
                }

                var storedHash = reader["PasswordHash"] as string ?? string.Empty;
                var fullName = reader["FullName"] as string ?? string.Empty;
                // Default role to 'admin' if the DB does not store roles
                var role = "admin";

                // Verify bcrypt hash. If the DB contains a legacy plaintext password, accept it and migrate to bcrypt.
                bool verified = false;
                try
                {
                    verified = BCrypt.Net.BCrypt.Verify(password ?? string.Empty, storedHash);
                }
                catch
                {
                    verified = false;
                }

                // If verification failed but storedHash equals the plaintext password (legacy data), migrate it to bcrypt
                if (!verified && !string.IsNullOrEmpty(storedHash) && storedHash == (password ?? string.Empty))
                {
                    verified = true;
                    try
                    {
                    var newHash = BCrypt.Net.BCrypt.HashPassword(password ?? string.Empty);
                        using var updateCmd = new SqlCommand("UPDATE Users SET PasswordHash = @PasswordHash WHERE UserName = @UserName", conn);
                        updateCmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = newHash;
                        updateCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value = userName;
                        updateCmd.ExecuteNonQuery();
                        // replace storedHash in-memory
                        storedHash = newHash;
                    }
                    catch (SqlException sex)
                    {
                        _logger?.LogError(sex, "Failed to migrate plaintext password to bcrypt for user {UserName}", userName);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Unexpected error migrating password for user {UserName}", userName);
                    }
                }

                if (verified)
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    // store the username in session (kept key name UserEmail for backwards compatibility)
                    HttpContext.Session.SetString("UserEmail", userName);
                    HttpContext.Session.SetString("UserFullName", fullName);
                    HttpContext.Session.SetString("UserRole", role);
                    return RedirectToAction("Index");
                }

                ViewBag.Error = "Invalid email or password";
                return View();
            }
            catch (SqlException sqlEx)
            {
                _logger?.LogError(sqlEx, "Database error during login for user {UserName}", userName);
                ViewBag.Error = "A database error occurred. Please try again later.";
                return View();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error during login for user {UserName}", userName);
                ViewBag.Error = "An unexpected error occurred. Please try again.";
                return View();
            }
        }

        // GET: Show Sign Up page
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Handle Sign Up
        [HttpPost]
        public IActionResult SignUp(string fullName, string email, string password, string confirmPassword)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                ViewBag.Error = "Please provide name, email and password.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View();
            }

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using var conn = new SqlConnection(connStr);
                conn.Open();

                // Check if user name already exists (table uses UserName)
                using (var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE UserName = @UserName", conn))
                {
                    checkCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value = email.Trim();
                    var exists = Convert.ToInt32(checkCmd.ExecuteScalar() ?? 0) > 0;
                    if (exists)
                    {
                        ViewBag.Error = "An account with that username/email already exists.";
                        return View();
                    }
                }

                // Hash password (bcrypt)
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                // Default role to 'admin'
                var role = "admin";
                var createdAt = DateTime.UtcNow;

                // Insert without Role column (some DB schemas don't have Role)
                using var insertCmd = new SqlCommand(
                    "INSERT INTO Users (UserName, PasswordHash, FullName, CreatedAt) VALUES (@UserName, @PasswordHash, @FullName, @CreatedAt)", conn);
                insertCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value = email.Trim();
                insertCmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = passwordHash;
                insertCmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 200).Value = fullName.Trim();
                insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value = createdAt;

                insertCmd.ExecuteNonQuery();

                // Auto-login after successful sign up
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserEmail", email.Trim());
                HttpContext.Session.SetString("UserFullName", fullName.Trim());
                HttpContext.Session.SetString("UserRole", role);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Sign up failed: " + ex.Message;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



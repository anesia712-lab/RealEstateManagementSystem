using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using RealEstateManagementSystem.Controllers;
using RealEstateManagementSystem.Models;
using System.Diagnostics;

namespace RealEstateManagementSystem.Controllers
{
    public class HomeController : Controller
    {
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
            if (email == "admin@realtor.com" && password == "1234")
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserEmail", email);
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
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
            // Check if passwords match
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View();
            }

            // Store user info in session (auto-login after sign up)
            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("UserFullName", fullName);

            // Redirect to home page
            return RedirectToAction("Index");
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



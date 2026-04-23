using Microsoft.AspNetCore.Mvc;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;

namespace RealEstateManagementSystem.Controllers
{
    public class PropertiesController : Controller
    {
        [HttpGet]
        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Add(Property property)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            PropertyService.Add(property);
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            var properties = PropertyService.GetAll();
            return View(properties);
        }
    }
}
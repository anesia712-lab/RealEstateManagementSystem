using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;

namespace RealEstateManagementSystem.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly PropertyService _propertyService;

        public PropertiesController(PropertyService propertyService)
        {
            _propertyService = propertyService;
        }

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

            if (property != null)
            {
                _propertyService.Add(property);
            }

            // Redirect to Index page to see the list
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            var properties = _propertyService.GetAll();
            return View(properties);
        }

        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            var property = _propertyService.GetById(id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }
    }
}

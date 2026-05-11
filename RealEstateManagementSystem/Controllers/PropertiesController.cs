using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RealEstateManagementSystem.Infrastructure;

namespace RealEstateManagementSystem.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly PropertyService _propertyService;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(PropertyService propertyService, ILogger<PropertiesController> logger)
        {
            _propertyService = propertyService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to add a property.";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Add(Property? property)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to add a property.";
                return RedirectToAction("Login", "Home");
            }

            property ??= new Property();
            if (!ModelState.IsValid)
                return View(property);

            try
            {
                _propertyService.Add(property);
                TempData["SuccessMessage"] = "Property saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error saving property");
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return View(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error saving property");
                TempData["ErrorMessage"] = FriendlyErrors.Describe(ex);
                return View(property);
            }
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to view properties.";
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var properties = _propertyService.GetAll();
                return View(properties);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error listing properties");
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return View(new List<Property>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error listing properties");
                TempData["ErrorMessage"] = FriendlyErrors.Describe(ex);
                return View(new List<Property>());
            }
        }

        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to view property details.";
                return RedirectToAction("Login", "Home");
            }

            if (id <= 0)
                return NotFound();

            try
            {
                var property = _propertyService.GetById(id);
                if (property == null)
                    return NotFound();

                return View(property);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error loading property {PropertyId}", id);
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

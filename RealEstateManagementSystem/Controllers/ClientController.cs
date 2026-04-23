using Microsoft.AspNetCore.Mvc;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;

namespace RealEstateManagementSystem.Controllers
{
    public class ClientController : Controller
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
        public IActionResult Add(Client client)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            ClientService.Add(client);
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            var client = ClientService.GetAll();
            return View(client);
        }
    }
}

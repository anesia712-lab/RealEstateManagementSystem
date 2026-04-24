using Microsoft.AspNetCore.Mvc;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;

namespace RealEstateManagementSystem.Controllers
{
    public class ClientsController : Controller
    {
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Client client)
        {
            if (client != null)
            {
                ClientService.Add(client);
            }
            // Redirect to Index page to see the list
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var clients = ClientService.GetAll();
            return View(clients);
        }
    }
}



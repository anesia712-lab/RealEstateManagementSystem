using Microsoft.AspNetCore.Mvc;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;

namespace RealEstateManagementSystem.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ClientService _clientService;

        public ClientsController(ClientService clientService)
        {
            _clientService = clientService;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Client client)
        {
            if (client != null)
            {
                _clientService.Add(client);
            }
            // Redirect to Index page to see the list
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var clients = _clientService.GetAll();
            return View(clients);
        }

        public IActionResult Details(int id)
        {
            var client = _clientService.GetById(id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }
    }
}



using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RealEstateManagementSystem.Infrastructure;

namespace RealEstateManagementSystem.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ClientService _clientService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(ClientService clientService, ILogger<ClientsController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to add a client.";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Add(Client? client)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to add a client.";
                return RedirectToAction("Login", "Home");
            }

            client ??= new Client();
            if (!ModelState.IsValid)
                return View(client);

            try
            {
                _clientService.Add(client);
                TempData["SuccessMessage"] = "Client saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error saving client");
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return View(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error saving client");
                TempData["ErrorMessage"] = FriendlyErrors.Describe(ex);
                return View(client);
            }
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to view clients.";
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var clients = _clientService.GetAll();
                return View(clients);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error listing clients");
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return View(new List<Client>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error listing clients");
                TempData["ErrorMessage"] = FriendlyErrors.Describe(ex);
                return View(new List<Client>());
            }
        }

        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to view client details.";
                return RedirectToAction("Login", "Home");
            }

            if (id <= 0)
                return NotFound();

            try
            {
                var client = _clientService.GetById(id);
                if (client == null)
                    return NotFound();

                return View(client);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error loading client {ClientId}", id);
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

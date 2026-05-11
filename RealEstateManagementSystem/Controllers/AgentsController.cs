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
    public class AgentsController : Controller
    {
        private readonly AgentService _agentService;
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(AgentService agentService, ILogger<AgentsController> logger)
        {
            _agentService = agentService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to add an agent.";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Add(Agent? agent)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to add an agent.";
                return RedirectToAction("Login", "Home");
            }

            agent ??= new Agent();
            if (!ModelState.IsValid)
                return View(agent);

            try
            {
                _agentService.Add(agent);
                TempData["SuccessMessage"] = "Agent saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error saving agent");
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return View(agent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error saving agent");
                TempData["ErrorMessage"] = FriendlyErrors.Describe(ex);
                return View(agent);
            }
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to view agents.";
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var agents = _agentService.GetAll();
                return View(agents);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error listing agents");
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return View(new List<Agent>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error listing agents");
                TempData["ErrorMessage"] = FriendlyErrors.Describe(ex);
                return View(new List<Agent>());
            }
        }

        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["InfoMessage"] = "Please sign in to view agent details.";
                return RedirectToAction("Login", "Home");
            }

            if (id <= 0)
                return NotFound();

            try
            {
                var agent = _agentService.GetById(id);
                if (agent == null)
                    return NotFound();

                return View(agent);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error loading agent {AgentId}", id);
                TempData["ErrorMessage"] = FriendlyErrors.DescribeSql(sqlEx);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

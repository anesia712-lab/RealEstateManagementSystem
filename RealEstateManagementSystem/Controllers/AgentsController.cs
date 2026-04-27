using Microsoft.AspNetCore.Mvc;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;
using System;
using Microsoft.AspNetCore.Http;

namespace RealEstateManagementSystem.Controllers
{
    public class AgentsController : Controller
    {
        private readonly AgentService _agentService;

        public AgentsController(AgentService agentService)
        {
            _agentService = agentService;
        }

        // Show Add Agent Form
        [HttpGet]
        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // Save new agent
        [HttpPost]
        public IActionResult Add(Agent agent)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            if (!ModelState.IsValid)
            {
                // Return view with validation messages
                return View(agent);
            }

            try
            {
                _agentService.Add(agent);
                TempData["SuccessMessage"] = "Agent added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Minimal error handling: show friendly message
                TempData["ErrorMessage"] = "Failed to add agent: " + ex.Message;
                return View(agent);
            }
        }

        // Show all agents
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            var agents = _agentService.GetAll();
            return View(agents);
        }

        // GET: /Agents/Details/{id}
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            var agent = _agentService.GetById(id);
            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }
    }
}


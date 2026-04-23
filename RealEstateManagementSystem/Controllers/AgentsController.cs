using Microsoft.AspNetCore.Mvc;
using RealEstateManagementSystem.Models;
using RealEstateManagementSystem.Services;

namespace RealEstateManagementSystem.Controllers
{
    public class AgentsController : Controller
    {
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

            AgentService.Add(agent);
            return RedirectToAction("Index");
        }

        // Show all agents
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            var agents = AgentService.GetAll();
            return View(agents);
        }
    }
}


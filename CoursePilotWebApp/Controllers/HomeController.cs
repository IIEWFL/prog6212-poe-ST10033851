using CoursePilotWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoursePilotWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Create a new instance of the ErrorViewModel and pass the RequestId,
            // which is either the current activity's Id or the HttpContext's TraceIdentifier.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using System.Diagnostics;
using GestionMedical.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionMedical.Controllers

{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize] // Ensure only authenticated users can access
        public IActionResult Index()
        {
            // Check if the logged-in user is the predefined admin
            if (User.Identity?.Name != "admin@gmail.com") // admin email
            {
                return Forbid(); // Return a 403 Forbidden response if not the admin
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult PublicIndex()
        {
            return View();
        }


        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

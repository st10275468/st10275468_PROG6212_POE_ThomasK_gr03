using Microsoft.AspNetCore.Mvc;
using st10275468_PROG6212_POE_ThomasK_gr03.Models;
using st10275468_PROG6212_POE_ThomasK_gr03.Controllers;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using st10275468_PROG6212_POE_ThomasK_gr03.Data;

namespace st10275468_PROG6212_POE_ThomasK_gr03.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ContractManagementContext _context;

        public HomeController(ILogger<HomeController> logger, ContractManagementContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SubmitClaims()
        {
            var fuserID = HttpContext.Session.GetInt32("userID");
            if (fuserID == null)
            {
                TempData["ErrorMessage"] = "User is not logged in.";
                return RedirectToAction("Index", "Home");
            }

           
            var Claims = _context.Claims
                .Where(c => c.userID == (int)fuserID)
                .ToList();

            return View(Claims);

           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

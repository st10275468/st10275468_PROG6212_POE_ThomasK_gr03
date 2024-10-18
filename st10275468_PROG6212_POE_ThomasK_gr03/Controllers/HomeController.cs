using Microsoft.AspNetCore.Mvc;
using st10275468_PROG6212_POE_ThomasK_gr03.Models;
using st10275468_PROG6212_POE_ThomasK_gr03.Controllers;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using st10275468_PROG6212_POE_ThomasK_gr03.Data;
using System.Drawing;

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
            var userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
             }
            var Claims = _context.Claims
                .Include(claim => claim.User)
                .Include(claim => claim.Documents)
                .Where(claim => claim.claimStatus == "Pending")
                .ToList();

            return View(Claims);
            
        }
        public IActionResult SubmitClaims()
        {
            var userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
            }

           
            var Claims = _context.Claims
                .Include(claim => claim.Documents)
                .Where(claim => claim.userID == (int)userID)
                .ToList();

            return View(Claims);

           
        }
        public IActionResult ApproveClaim(int claimID)
        {
            var Claim = _context.Claims.FirstOrDefault(claim => claim.claimID == claimID);
            if (Claim != null)
            {
                
                Claim.claimStatus = "Approved";
                _context.SaveChanges();
            }

            return RedirectToAction("Privacy"); 
        }

        public IActionResult DenyClaim(int claimID)
        {
            var Claim = _context.Claims.FirstOrDefault(claim => claim.claimID == claimID);
            if (Claim != null)
            {
                Claim.claimStatus = "Denied";
                _context.SaveChanges();
            }

            return RedirectToAction("Privacy");
        }

        public IActionResult DownloadDocument(int documentID)
        {
            var document = _context.Documents.FirstOrDefault(document => document.documentID == documentID);
            if (document == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", document.path); 
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", document.path);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

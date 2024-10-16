using Microsoft.AspNetCore.Mvc;
using st10275468_PROG6212_POE_ThomasK_gr03.Data;
using st10275468_PROG6212_POE_ThomasK_gr03.Models;
namespace st10275468_PROG6212_POE_ThomasK_gr03.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ContractManagementContext _context;

        public ClaimController(ContractManagementContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(DateTime claimMonth, int hoursWorked, decimal hourlyRate, IFormFileCollection supportingDocument)
        {
            if (ModelState.IsValid)
            {
                var fuserID = HttpContext.Session.GetString("userID");
                if (string.IsNullOrEmpty(fuserID) || !int.TryParse(fuserID, out int userID))
                {
                    
                    ModelState.AddModelError("", "User is not logged in or user ID is invalid.");
                    return View("Index"); 
                }
                var claim = new Claim
                {
                    claimMonth = claimMonth,
                    claimAmount = hourlyRate * hoursWorked,
                    submissionDate = DateTime.Now,
                    claimStatus = "Pending",
                    userID = userID,

                     };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                if (supportingDocument != null)
                {
                    foreach (var file in supportingDocument)
                    {
                        if (file.Length > 0)
                        {
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", file.FileName); 

                            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Documents")))
                            {
                                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Documents"));
                            }
                           
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            var document = new Document
                            {
                                path = file.FileName,
                                claimID = claim.claimID,
                            };
                            _context.Documents.Add(document);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("SubmitClaims", "Home");

            }
            return RedirectToAction("SubmitClaims", "Home");

        }

        [HttpGet]
        public IActionResult SubmitClaims()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

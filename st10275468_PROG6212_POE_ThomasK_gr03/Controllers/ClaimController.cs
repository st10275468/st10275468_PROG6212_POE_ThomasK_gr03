using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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
                var fuserID = HttpContext.Session.GetInt32("userID");
                if (fuserID == null )
                {
                    TempData["ErrorMessage"] = "Invalid login or userID";
                    return RedirectToAction("Index", "Home"); 
                }
                var claim = new Claim
                {
                    claimMonth = claimMonth,
                    claimAmount = hourlyRate * hoursWorked,
                    submissionDate = DateTime.Now,
                    claimStatus = "Pending",
                    userID = (int)fuserID,

                     };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your claim has been submitted successfully";


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
            return RedirectToAction("SubmitClaims","Home");

        }

       


        public IActionResult Index()
        {
            return View();
        }
    }
}

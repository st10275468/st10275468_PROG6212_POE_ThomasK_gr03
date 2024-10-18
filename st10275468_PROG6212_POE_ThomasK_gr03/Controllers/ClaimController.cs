/*  OpenAI.2024. Chat-GPT(Version 3.5).[Large language model]. Available at: https://chat.openai.com/[Accessed: 17 October 2024]. */
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

        /// <summary>
        /// Method created that allows lectures to submit their monthly claims using input fields. The claims are then saved into a database
        /// </summary>
        /// <param name="claimMonth"></param>
        /// <param name="hoursWorked"></param>
        /// <param name="hourlyRate"></param>
        /// <param name="supportingDocument"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SubmitClaim(DateTime claimMonth, int hoursWorked, decimal hourlyRate, IFormFileCollection supportingDocument)
        {
            if (ModelState.IsValid)
            {
                //Getting the userID from the session and making sure it is valid otherwise it will take them to the homescreen and prompt them
                var fuserID = HttpContext.Session.GetInt32("userID");
                if (fuserID == null )
                {
                    //Error handeling prompting the user that they are not logged in
                    TempData["ErrorMessage"] = "Invalid login or userID";
                    return RedirectToAction("Index", "Home"); 
                }
                var claim = new Claim
                {
                    //Creating a new claims with the inputted data
                    claimMonth = claimMonth,
                    claimAmount = hourlyRate * hoursWorked,
                    submissionDate = DateTime.Now,
                    claimStatus = "Pending",
                    userID = (int)fuserID,

                     };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();
                //Prompting the user that their claim was submitted
                TempData["SuccessMessage"] = "Your claim has been submitted successfully";

                //Optional to have a supporting document
                if (supportingDocument != null)
                {
                    foreach (var file in supportingDocument)
                    {
                        //If they add a document the document will be saved into a folder where it can be viewed by the lectures or admins
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

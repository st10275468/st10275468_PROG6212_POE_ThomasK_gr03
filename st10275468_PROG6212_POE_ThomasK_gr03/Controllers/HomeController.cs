/*  OpenAI.2024. Chat-GPT(Version 3.5).[Large language model]. Available at: https://chat.openai.com/[Accessed: 17 October 2024]. */
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
        private readonly ContractManagementContext _context;

        public HomeController(ContractManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GenerateInvoices()
        {
            //Retrieving the current user id
            int? userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                //Error handeling to make sure that only logged in users will be allowed on this page
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
            }
            var Claims = await _context.Claims
                .Include(claim => claim.User)
                .Include(claim => claim.Documents)
                .Where(claim => claim.claimStatus == "Approved")
                .ToListAsync();

            return View(Claims);
        }
        public IActionResult GenerateInvoice(int claimID)
        {
            var claim = _context.Claims.Include(c => c.User).FirstOrDefault(c => c.claimID == claimID);

            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction("GenerateInvoices");
            }

            string invoiceDetails = $"Invoice for Claim #{claim.claimID}\n" +
                                    $"Lecturer: {claim.User.name} {claim.User.surname}\n" +
                                    $"Claim Amount: R {claim.claimAmount}\n" +
                                    $"Status: {claim.claimStatus}\n" +
                                    $"Submission Date: {claim.submissionDate:dd MMM yyyy}\n";

            string invoicePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoices", $"Invoice_{claim.claimID}.txt");
            System.IO.File.WriteAllText(invoicePath, invoiceDetails);

            claim.claimStatus = "Processed - Invoice created";
            _context.SaveChanges(); 

            TempData["SuccessMessage"] = $"Invoice for Claim #{claim.claimID} generated successfully.";

            var fileBytes = System.IO.File.ReadAllBytes(invoicePath);
          
            var approvedClaims = _context.Claims
                .Include(claim => claim.User)
                .Include(claim => claim.Documents)
                .Where(claim => claim.claimStatus == "Approved")
                .ToList();

           
            return View("GenerateInvoices", approvedClaims); 
        }


        /// <summary>
        /// Method that passes all the claims and allows them to be displayed to the Admins who can manage them
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Privacy()
        {
            //Retrieving the current user id
            int? userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                //Error handeling to make sure that only logged in users will be allowed on this page
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
             }
            //Creating a list of all the claims and passing it to the view to be displayed
            var Claims = await _context.Claims
                .Include(claim => claim.User)
                .Include(claim => claim.Documents)
                .Where(claim => claim.claimStatus == "Pending")
                .ToListAsync();

            return View(Claims);
            
        }
        /// <summary>
        /// Method created that will display the users claims specific to the user logged in
        /// </summary>
        /// <returns></returns>
        public IActionResult SubmitClaims()
        {
            //Retrieving the current user id
            var userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                //Error handeling if the user is not logged in
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
            }
            //Making a list with all the users claims associated with that specific user ID
            var Claims = _context.Claims
                .Include(claim => claim.Documents)
                .Where(claim => claim.userID == (int)userID)
                .ToList();
            //passing the list to the view to be displayed
            return View(Claims);

           
        }

        /// <summary>
        /// Method created to allow the Admins to approve claims when the button is clicked
        /// </summary>
        /// <param name="claimID"></param>
        /// <returns></returns>
        public IActionResult ApproveClaim(int claimID)
        {
            var Claim = _context.Claims.FirstOrDefault(claim => claim.claimID == claimID);
            if (Claim != null)
            {
                //Changing the claim status to approved once the button is clicked
                Claim.claimStatus = "Approved";
                if (Claim.claimVerification == "Failed - Under Review")
                {
                    Claim.claimVerification = "Reviewed - passed manual verification"; 
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Privacy"); 
        }

        /// <summary>
        /// Method created to allow the admins to deny claims 
        /// </summary>
        /// <param name="claimID"></param>
        /// <returns></returns>
        public IActionResult DenyClaim(int claimID)
        {
            var Claim = _context.Claims.FirstOrDefault(claim => claim.claimID == claimID);
            if (Claim != null)
            {
                //Changing the claim status to denied once the deny button is clicked
                Claim.claimStatus = "Denied";
                if (Claim.claimVerification == "Failed - Under Review")
                {
                    Claim.claimVerification = "Reviewed - Failed manual verification";
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Privacy");
        }


        /// <summary>
        /// Method created to allow the admins to download and view the documents associated with the specific claims
        /// </summary>
        /// <param name="documentID"></param>
        /// <returns></returns>
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

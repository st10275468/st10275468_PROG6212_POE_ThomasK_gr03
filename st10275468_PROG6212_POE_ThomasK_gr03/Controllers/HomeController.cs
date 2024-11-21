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

        /// <summary>
        /// Method opens the ManageLecturers View and passes a list of 
        /// all the users with a role as a "lecturer" to be displayed on it
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageLecturers()
        {
            //Retrieving the current user id
            int? userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                //Error handeling to make sure that only logged in users will be allowed on this page
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
            }
            //Creating a list of users with a role as a lecturer
            var lecturers =  await _context.Users
                .Where(user => user.role == "Lecturer")
                .ToListAsync();

            //Passing the list to the view
            return View(lecturers);
        }

        /// <summary>
        /// Method that opens the EditLecturerDetails view when the button is pressed.
        /// This method passes the specific user(Lecturer) details to the view so they can
        /// be edited
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditLecturerDetails(int userID)
        {
            int? fuserID = HttpContext.Session.GetInt32("userID");
            if (fuserID == null)
            {
                //Error handeling to make sure that only logged in users will be allowed on this page
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
            }
            //Creating user with the details of the specific lecturer that is being edited  
            var lecturer = await _context.Users.FirstOrDefaultAsync(user => user.userID == userID && user.role == "Lecturer");

            if (lecturer != null)
            {
                //Passing the specific lecturer details to the view   
                return View(lecturer);
            }
            else
            {
                TempData["ErrorMessage"] = "Lecturer not found or you can't access this page.";
                return RedirectToAction("ManageLecturers");
            }
        }


        /// <summary>
        /// Method created that takes in the updated details of the lecturer and saves the changes to the database
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateLecturerDetails(int userID, string name, string surname, string email)
        {
            int? fuserID = HttpContext.Session.GetInt32("userID");
            if (fuserID == null)
            {
                //Error handeling to make sure that only logged in users will be allowed on this page
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
            }
            //Getting the lecturer details so that they can be displayed and edited in the view
            var lecturer = await _context.Users.FirstOrDefaultAsync(user => user.userID == userID && user.role == "Lecturer");
           
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers");
            }
            //Setting the details as the new edited values
            lecturer.name = name;
            lecturer.surname = surname;
            lecturer.email = email;

            //Saving the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageLecturers");
        }


        /// <summary>
        /// Method created that allows admins to delete the lecturer account completely
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteLecturer(int userID)
        {
            //Gets the details of the user with that specific ID
            var user = await _context.Users.FirstOrDefaultAsync(user => user.userID == userID);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers"); 
            }
            //Removing that user from the database
            _context.Users.Remove(user);
            //Saving the changes
            await _context.SaveChangesAsync();

            
            return RedirectToAction("ManageLecturers");
        }


        /// <summary>
        /// Method that opens the GenerateInvoices view and passes the list of claims that are approved to it
        /// </summary>
        /// <returns></returns>
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
            //Creating a new list of all the claims which the status is approved
            var Claims = await _context.Claims
                .Include(claim => claim.User)
                .Include(claim => claim.Documents)
                .Where(claim => claim.claimStatus == "Approved")
                .ToListAsync();
            //Returning the list to the view to be displayed
            return View(Claims);
        }


        /// <summary>
        /// Method created that is triggered when the generate invoice button is clicked.
        /// This method uses the specific claimID to recieve the details of the claim and generate an invoice of it.
        /// The invoice is saved in a folder called invoices which can be downloaed by the lecturers
        /// </summary>
        /// <param name="claimID"></param>
        /// <returns></returns>
        public IActionResult GenerateInvoice(int claimID)
        {
            //Getting the details of the specific claim
            var claim = _context.Claims.Include(claim => claim.User).FirstOrDefault(claim => claim.claimID == claimID);

            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction("GenerateInvoices");
            }
             //Creating the invoice layout
            string invoiceData = $"-------CLAIM INVOICE-------\n" +
                                    $"Invoice for Claim #{claim.claimID}\n" +
                                    $"Lecturer: {claim.User.name} {claim.User.surname}\n" +
                                    $"Email: {claim.User.email}\n" +
                                    $"Claim Amount: R {claim.claimAmount}\n" +
                                    $"Claim Month: {claim.claimMonth: MMM yyyy}\n" +
                                    $"Status: {claim.claimStatus}\n" +
                                    $"Verification: {claim.claimVerification}\n" +
                                    $"Submission Date: {claim.submissionDate:dd MMM yyyy}\n";
            //Creating the path and invoice name
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Invoices", $"Invoice_{claim.claimID}.txt");
            System.IO.File.WriteAllText(path, invoiceData);
            //Changing the status of the claim from approved to processed - invoice created
            claim.claimStatus = "Processed - Invoice created";
            //Saving changes
            _context.SaveChanges(); 

            TempData["SuccessMessage"] = $"Invoice for Claim #{claim.claimID} generated successfully.";

            var fileBytes = System.IO.File.ReadAllBytes(path);
          //Regenerating the list of approved claims so that the table is refreshed after the invoices are generated
            var approvedClaims = _context.Claims
                .Include(claim => claim.User)
                .Include(claim => claim.Documents)
                .Where(claim => claim.claimStatus == "Approved")
                .ToList();

           
            return View("GenerateInvoices", approvedClaims); 
        }


        /// <summary>
        /// Method triggered by the Download invoice button that is shown under the My invoices as a lecturer.
        /// This method allows the lecturer to download and view their invoices for all the approved claims that have been processed
        /// </summary>
        /// <param name="claimID"></param>
        /// <returns></returns>
        public IActionResult DownloadInvoice(int claimID)
        {
            //Fetches the details of that specific claim
            var claim = _context.Claims.Include(claim => claim.User).FirstOrDefault(claim => claim.claimID == claimID);

            if (claim == null)
            { //Error checking if the claim doesnt exist
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction("SubmitClaims");
            }
            //Error checking if it has not invoice
            if (claim.claimStatus != "Processed - Invoice created")
            {
                TempData["ErrorMessage"] = "Invoice has not been generated for this claim.";
                return RedirectToAction("SubmitClaims");
            }
            //Getting the path for the specific invoice relating to that claim
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Invoices", $"Invoice_{claim.claimID}.txt");

            if (!System.IO.File.Exists(path))
            {
                TempData["ErrorMessage"] = "Invoice file not found.";
                return RedirectToAction("SubmitClaims");
            }
            
            var fileBytes = System.IO.File.ReadAllBytes(path);
            var fileName = Path.GetFileName(path);
            //Returning the invoice as a download
            return File(fileBytes, "application/octet-stream", fileName);
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
                    Claim.claimVerification = "Passed manual verification"; 
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
                    Claim.claimVerification = "Failed manual verification";
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

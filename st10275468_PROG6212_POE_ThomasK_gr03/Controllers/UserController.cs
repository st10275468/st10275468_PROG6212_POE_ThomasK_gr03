/*  OpenAI.2024. Chat-GPT(Version 3.5).[Large language model]. Available at: https://chat.openai.com/[Accessed: 20 November 2024]. */
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using st10275468_PROG6212_POE_ThomasK_gr03.Data;
using st10275468_PROG6212_POE_ThomasK_gr03.Models;

namespace st10275468_PROG6212_POE_ThomasK_gr03.Controllers
{
    public class UserController : Controller
    {
        private readonly ContractManagementContext _context;

        public UserController(ContractManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method created that allows a new user to create an account before login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                //Adding the new user
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                //Prompting the user that they created an account
                TempData["SuccessMessage"] = "Account created successfully.";
                
                return RedirectToAction("Index", "Home");
            }
            
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = string.Join(", ", errors); 

            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Method created to allow the user to login once the account has been created
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = await _context.Users
                //Checking if the user exists and if their email and password match
           .FirstOrDefaultAsync(u => u.email == user.email && u.password == user.password);

            if (existingUser != null)
            {
               
                HttpContext.Session.SetInt32("userID", existingUser.userID);
                HttpContext.Session.SetString("Role", existingUser.role);
                HttpContext.Session.SetString("Name" , existingUser.name);
                
                TempData["SuccessMessage"] = ("Welcome back " + existingUser.name + "!" );
                //Prompting the user that they have logged in
              
                
            }
            TempData["Message"] = ("Failed Login attempt! Please create an account or try again");
            //Prompting the user if they failed to login.
            return RedirectToAction("Index", "Home");
        }

        
        /// <summary>
        /// Method created that logs the user out be clearing the userID and related information and taking them back to the home page
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = ("Successfully logged out");
            return RedirectToAction("Index", "Home");
        }


    }

}

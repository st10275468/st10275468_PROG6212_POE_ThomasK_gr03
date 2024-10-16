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

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Account created successfully. You can now log in.";
                return RedirectToAction("Index", "Home");
            }
            
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = string.Join(", ", errors); 

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = await _context.Users
           .FirstOrDefaultAsync(u => u.email == user.email && u.password == user.password);

            if (existingUser != null)
            {
               
                HttpContext.Session.SetString("userID", existingUser.userID.ToString());
                HttpContext.Session.SetString("Role", existingUser.role);
                HttpContext.Session.SetString("Name" , existingUser.name);
                
                TempData["SuccessMessage"] = ("Welcome back " + existingUser.name + "! As a " + existingUser.role + " . ");
                if (existingUser.role == "Lecturer")
                {
                    return RedirectToAction("Index", "Home"); 
                }
                if (existingUser.role == "Program Coordinator" || existingUser.role == "Academic Manager")
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return RedirectToAction("Index", "Home");
        }

        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = ("Successfully logged out");
            return RedirectToAction("Index", "Home");
        }


    }

}

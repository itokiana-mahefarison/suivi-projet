using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace Backoffice.Pages.Users
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public UserInputModel NewUser { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public List<User> Users { get; set; }

        public void OnGet()
        {
            // TODO: Get roles from database
            Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "admin", Text = "Administrator" },
                new SelectListItem { Value = "manager", Text = "Project Manager" },
                new SelectListItem { Value = "user", Text = "User" }
            };

            // TODO: Get users from database
            Users = new List<User>();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Save user to database
            return RedirectToPage();
        }
    }

    public class UserInputModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        public string Phone { get; set; }
    }
} 
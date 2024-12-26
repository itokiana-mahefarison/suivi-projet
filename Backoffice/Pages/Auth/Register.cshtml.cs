using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Backoffice.Config.Database;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        [BindProperty]
        [Display(Name = "First name")]
        public string? Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [BindProperty]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [BindProperty]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [BindProperty]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [BindProperty]
        [Display(Name = "City")]
        public string? City { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "You must accept the terms and conditions")]
        [Display(Name = "Terms and Conditions")]
        public bool AcceptTerms { get; set; }

        public string? ErrorMessage { get; set; }

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!AcceptTerms)
            {
                ModelState.AddModelError("AcceptTerms", "You must accept the terms and conditions");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                ErrorMessage = string.Join(" ", errors);
                return Page();
            }

            // Vérifier si l'email existe déjà
            if (_context.Users.Any(u => u.Email == Email))
            {
                ErrorMessage = "This email is already registered";
                return Page();
            }

            var user = new User
            {
                Name = Name,
                Email = Email,
                Password = Password, // Note: Il faudrait hasher le mot de passe dans une vraie application
                Address = Address,
                Country = Country,
                Phone = Phone,
                City = City,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Redirection vers la page de connexion avec un message de succès
                TempData["SuccessMessage"] = "Account created successfully. Please log in.";
                return RedirectToPage("/Auth/Login");
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while creating your account. Please try again.";
                return Page();
            }
        }
    }
}

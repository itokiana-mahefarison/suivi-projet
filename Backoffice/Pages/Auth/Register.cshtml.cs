using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Backoffice.Config.Database;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using Backoffice.Services.Interfaces;

namespace Backoffice.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

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

        public RegisterModel(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
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

            var user = new User
            {
                Name = Name,
                Email = Email,
                Password = Password,
                Address = Address,
                Country = Country,
                Phone = Phone,
                City = City,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                if (await _authService.RegisterUserAsync(user))
                {
                    await _authService.SignInAsync(user, HttpContext);
                    return RedirectToPage("/Index");
                }

                ErrorMessage = "This email is already registered";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while creating your account. Please try again.";
                return Page();
            }
        }
    }
}

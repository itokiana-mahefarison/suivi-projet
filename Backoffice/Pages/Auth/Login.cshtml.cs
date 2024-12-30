using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Backoffice.Config.Database;
using Microsoft.EntityFrameworkCore;
using Backoffice.Services.Interfaces;

namespace Backoffice.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;
        
        [BindProperty]
        public string Email { get; set; }
        
        [BindProperty]
        public string Password { get; set; }
        
        public string ErrorMessage { get; set; }

        public LoginModel(IAuthService authService, AppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == Email);

            var validatedCredetials = await _authService.ValidateLoginAsync(Email, Password);

            if (user == null || !validatedCredetials)
            {
                ErrorMessage = "Invalid email or password";
                return Page();
            }

            await _authService.SignInAsync(user, HttpContext);
            return RedirectToPage("/Index");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Backoffice.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        
        [BindProperty]
        public string Email { get; set; }
        
        [BindProperty]
        public string Password { get; set; }
        
        public string ErrorMessage { get; set; }

        public LoginModel(AppDbContext context)
        {
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
                .FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);

            if (user == null)
            {
                ErrorMessage = "Invalid email or password";
                return Page();
            }

            // TODO: Implement proper authentication
            return RedirectToPage("/Index");
        }
    }
}

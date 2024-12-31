using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backoffice.Config.Database;
using Shared.Utils.Pagination;

namespace Backoffice.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private const int DefaultPageSize = 10;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserInputModel NewUser { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public PaginatedList<User> Users { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = DefaultPageSize;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        public async Task OnGetAsync()
        {
            Roles = await _context.Roles.Select(r => new SelectListItem 
            { 
                Value = r.Id.ToString(), 
                Text = r.Label 
            }).ToListAsync();

            var query = _context.Users
                .Include(u => u.Role)
                .Where(u => u.DeletedAt == null);

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(u => u.Name.Contains(SearchTerm) || u.Email.Contains(SearchTerm));
            }

            if (!string.IsNullOrEmpty(RoleFilter))
            {
                query = query.Where(u => u.RoleId.ToString() == RoleFilter);
            }

            Users = await query.ToPagedListAsync(PageNumber, PageSize);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // Recharger les données
                return Page();
            }

            // Création du nouvel utilisateur
            var user = new User
            {
                Name = NewUser.Name,
                Email = NewUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword("0000"),
                Phone = NewUser.Phone,
                RoleId = Int32.Parse(NewUser.Role)
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, UserInputModel updatedUser)
        {

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;
            user.RoleId = Int32.Parse(updatedUser.Role);
            user.HourlyRate = updatedUser.HourlyRate;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }

    public class UserInputModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        public string Phone { get; set; }

        [Display(Name = "Hourly Rate")]
        [Range(0, double.MaxValue, ErrorMessage = "Hourly rate must be a positive number")]
        public double? HourlyRate { get; set; }
    }
} 
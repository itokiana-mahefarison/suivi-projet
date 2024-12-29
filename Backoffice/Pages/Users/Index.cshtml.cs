using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backoffice.Config.Database;

namespace Backoffice.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserInputModel NewUser { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public List<User> Users { get; set; }

        public async Task OnGetAsync()
        {
            Roles = await _context.Roles.Select(r => new SelectListItem 
            { 
                Value = r.Id.ToString(), 
                Text = r.Label 
            }).ToListAsync();

            // Récupération des utilisateurs avec leurs rôles
            Users = await _context.Users.Include(u => u.Role).ToListAsync();
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
    }
} 
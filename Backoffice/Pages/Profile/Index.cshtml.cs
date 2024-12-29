using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System.Security.Claims;
using Backoffice.Config.Database;

namespace Backoffice.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Récupérer l'ID de l'utilisateur connecté
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Récupérer les informations de l'utilisateur avec son rôle
            CurrentUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (CurrentUser == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userToUpdate = await _context.Users.FindAsync(CurrentUser.Id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            // Mise à jour des propriétés modifiables
            userToUpdate.Name = CurrentUser.Name;
            userToUpdate.Email = CurrentUser.Email;
            userToUpdate.Phone = CurrentUser.Phone;
            userToUpdate.Address = CurrentUser.Address;
            userToUpdate.City = CurrentUser.City;
            userToUpdate.Country = CurrentUser.Country;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully";
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Error updating profile");
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Please check your inputs");
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
            {
                return NotFound();
            }

            // Vérifier l'ancien mot de passe
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                ModelState.AddModelError("", "Current password is incorrect");
                return Page();
            }

            // Hasher et enregistrer le nouveau mot de passe
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Password changed successfully";
            }
            catch
            {
                ModelState.AddModelError("", "Error changing password");
                return Page();
            }

            return RedirectToPage();
        }
    }
} 
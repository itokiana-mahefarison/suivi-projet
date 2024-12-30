using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Frontoffice.Config.Database;
using Shared.Models;
using BCrypt.Net;

namespace Frontoffice.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly DatabaseConnection _db;

        public ProfileController(DatabaseConnection db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

            var query = "SELECT * FROM Users WHERE Id = @UserId";
            using (var reader = _db.ExecuteReader(query, new SqlParameter("@UserId", userId)))
            {
                if (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Name = !reader.IsDBNull(reader.GetOrdinal("Name")) ? reader.GetString(reader.GetOrdinal("Name")) : null,
                        Phone = !reader.IsDBNull(reader.GetOrdinal("Phone")) ? reader.GetString(reader.GetOrdinal("Phone")) : null,
                        Address = !reader.IsDBNull(reader.GetOrdinal("Address")) ? reader.GetString(reader.GetOrdinal("Address")) : null,
                        City = !reader.IsDBNull(reader.GetOrdinal("City")) ? reader.GetString(reader.GetOrdinal("City")) : null,
                        Country = !reader.IsDBNull(reader.GetOrdinal("Country")) ? reader.GetString(reader.GetOrdinal("Country")) : null,
                        Password = ""
                    };
                    return View(user);
                }
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(User model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

            var query = @"UPDATE Users 
                         SET Name = @Name, 
                             Phone = @Phone, 
                             Address = @Address, 
                             City = @City, 
                             Country = @Country
                         WHERE Id = @UserId";

            try
            {
                _db.ExecuteNonQuery(query,
                    new SqlParameter("@Name", (object?)model.Name ?? DBNull.Value),
                    new SqlParameter("@Phone", (object?)model.Phone ?? DBNull.Value),
                    new SqlParameter("@Address", (object?)model.Address ?? DBNull.Value),
                    new SqlParameter("@City", (object?)model.City ?? DBNull.Value),
                    new SqlParameter("@Country", (object?)model.Country ?? DBNull.Value),
                    new SqlParameter("@UserId", userId));

                TempData["SuccessMessage"] = "Profile updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update profile: " + ex.Message);
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(string currentPassword, string newPassword)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

            // Vérifier le mot de passe actuel
            var query = "SELECT Password FROM Users WHERE Id = @UserId";
            var currentHash = _db.ExecuteScalar(query, new SqlParameter("@UserId", userId))?.ToString();

            if (currentHash == null || !BCrypt.Net.BCrypt.Verify(currentPassword, currentHash))
            {
                ModelState.AddModelError("", "Current password is incorrect");
                return RedirectToAction(nameof(Index));
            }

            // Mettre à jour le mot de passe
            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            query = "UPDATE Users SET Password = @Password WHERE Id = @UserId";

            try
            {
                _db.ExecuteNonQuery(query,
                    new SqlParameter("@Password", newHash),
                    new SqlParameter("@UserId", userId));

                TempData["SuccessMessage"] = "Password updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update password: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 
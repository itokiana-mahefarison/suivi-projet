using Frontoffice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using Shared.Models;
using Frontoffice.Config.Database;
using Microsoft.Data.SqlClient;

namespace Frontoffice.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseConnection _db;

        public HomeController(DatabaseConnection db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

            var query = @"SELECT t.*, p.Title as ProjectName 
                         FROM Tasks t 
                         LEFT JOIN Projects p ON t.ProjectId = p.Id 
                         WHERE t.UserId = @UserId";
            
            var tasks = new List<Tasks>();
            using (var reader = _db.ExecuteReader(query, new SqlParameter("@UserId", userId)))
            {
                while (reader.Read())
                {
                    tasks.Add(new Tasks
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Description = !reader.IsDBNull(reader.GetOrdinal("Description")) ? 
                            reader.GetString(reader.GetOrdinal("Description")) : null,
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        EndDate = !reader.IsDBNull(reader.GetOrdinal("EndDate")) ? 
                            reader.GetDateTime(reader.GetOrdinal("EndDate")) : null,
                        Project = new Project 
                        { 
                            Title = !reader.IsDBNull(reader.GetOrdinal("ProjectName")) ? 
                                reader.GetString(reader.GetOrdinal("ProjectName")) : null 
                        }
                    });
                }
            }

            return View(tasks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTaskStatus([FromBody] TaskUpdateModel model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = "UPDATE Tasks SET Status = @Status WHERE Id = @Id AND UserId = @UserId";
            
            try
            {
                _db.ExecuteNonQuery(query, 
                    new SqlParameter("@Status", model.Status),
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", userId));
                
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class TaskUpdateModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
    }
}

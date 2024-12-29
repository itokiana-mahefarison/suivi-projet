using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Backoffice.Config.Database;
using Shared.Models;

namespace Backoffice.Pages.TasksManager
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TaskInputModel NewTask { get; set; }
        public List<Tasks> Tasks { get; set; }
        public List<SelectListItem> Statuses { get; set; }
        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> Projects { get; set; }

        public async Task OnGetAsync()
        {
            // Charger les tâches depuis la BDD
            Tasks = await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.ParentTask)
                .ToListAsync();
            
            await LoadSelectLists();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                return Page();
            }

            var task = new Tasks
            {
                Title = NewTask.Title,
                Description = NewTask.Description,
                StartDate = NewTask.StartDate,
                EndDate = NewTask.EndDate,
                Status = NewTask.Status,
                UserId = NewTask.AssignedToId,
                ProjectId = NewTask.ProjectId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateStatusTaskAsync([FromBody] TaskUpdateModel model)
        {
            var task = await _context.Tasks.FindAsync(model.Id);
            if (task == null) return NotFound();

            task.Status = model.Status;

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateTaskAsync([FromBody] TaskUpdateModel model)
        {
            var task = await _context.Tasks.FindAsync(model.Id);
            if (task == null) return NotFound();

            task.Title = model.Title;
            task.Description = model.Description;
            task.UserId = model.AssignedToId;
            task.StartDate = model.StartDate;
            task.EndDate = model.EndDate;
            task.Status = model.Status;

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        private async Task LoadSelectLists()
        {
            Statuses = new List<SelectListItem>
            {
                new SelectListItem("To Do", "todo"),
                new SelectListItem("In Progress", "in-progress"),
                new SelectListItem("Done", "done")
            };

            Users = await _context.Users
                .Select(u => new SelectListItem 
                { 
                    Value = u.Id.ToString(), 
                    Text = u.Name 
                })
                .ToListAsync();

            Projects = await _context.Projects
                .Select(p => new SelectListItem 
                { 
                    Value = p.Id.ToString(), 
                    Text = p.Title 
                })
                .ToListAsync();
        }
    }

    public class TaskInputModel
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Status { get; set; }
        public int? AssignedToId { get; set; }
        [Required(ErrorMessage = "Project required")]
        public int ProjectId { get; set; }
    }

    public class TaskUpdateModel
    {
        public int? Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int? AssignedToId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}

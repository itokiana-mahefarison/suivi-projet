using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Backoffice.Config.Database;
using Shared.Utils.Pagination;

namespace Backoffice.Pages.Projects
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProjectInputModel NewProject { get; set; }
        public List<SelectListItem> Clients { get; set; }
        public PaginatedList<Project> Projects { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            if (!Request.Query.ContainsKey("pageNumber"))
                PageNumber = 1;
            if (!Request.Query.ContainsKey("pageSize"))
                PageSize = 10;

            // Récupération des clients pour le dropdown
            Clients = await _context.Clients
                .Where(c => c.DeletedAt == null)  // On ne prend que les clients actifs
                .Select(c => new SelectListItem 
                { 
                    Value = c.Id.ToString(), 
                    Text = c.Name 
                })
                .ToListAsync();

            var query = _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Tasks)
                .Where(p => p.DeletedAt == null)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(p => 
                    p.Title.Contains(SearchTerm) || 
                    p.Description.Contains(SearchTerm));
            }

            var projects = await query.ToPagedListAsync(PageNumber, PageSize);

            foreach (var project in projects.Items)
            {
                project.Progress = project.Tasks.Any()
                    ? (double)project.Tasks.Count(t => t.Status == "done") / project.Tasks.Count * 100
                    : 0;
            }

            Projects = projects;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // Recharger les listes en cas d'erreur
                return Page();
            }

            Project project = new Project
            {
                Title = NewProject.Name,
                Description = NewProject.Description,
                Budget = NewProject.Budget,
                ClientId = int.Parse(NewProject.ClientId)
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            project.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, string title, string description, int clientId)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            project.Title = title;
            project.Description = description;
            project.ClientId = clientId;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }

    public class ProjectInputModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public Double? Budget { get; set; }
        
        [Required(ErrorMessage = "Client is required")]
        public string ClientId { get; set; }
    }
}

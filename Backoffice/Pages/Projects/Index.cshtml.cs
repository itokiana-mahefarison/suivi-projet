using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Backoffice.Config.Database;

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
        public List<Project> Projects { get; set; }

        public async Task OnGetAsync()
        {
            // Récupération des clients et conversion en SelectListItem
            Clients = await _context.Clients
                .Select(c => new SelectListItem 
                { 
                    Value = c.Id.ToString(), 
                    Text = c.Name 
                })
                .ToListAsync();

            // Récupération des projets avec leurs clients
            Projects = await _context.Projects
                .Include(p => p.Client)
                .ToListAsync();
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

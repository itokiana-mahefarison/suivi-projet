using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.Pages.Projects
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ProjectInputModel NewProject { get; set; }
        public List<SelectListItem> Clients { get; set; }
        public List<Project> Projects { get; set; }

        public void OnGet()
        {
            // TODO: Récupérer la liste des clients depuis la BDD
            Clients = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Client 1" },
                new SelectListItem { Value = "2", Text = "Client 2" }
            };

            // TODO: Récupérer la liste des projets depuis la BDD
            Projects = new List<Project>();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Sauvegarder le projet dans la BDD
            return RedirectToPage();
        }
    }

    public class ProjectInputModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal? Budget { get; set; }
        
        [Required(ErrorMessage = "Client is required")]
        public string ClientId { get; set; }
    }
}

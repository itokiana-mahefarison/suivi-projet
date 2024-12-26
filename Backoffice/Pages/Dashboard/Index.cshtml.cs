using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backoffice.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        public int ActiveProjects { get; set; } = 12;
        public int TotalUsers { get; set; } = 48;
        public int TotalTasks { get; set; } = 156;
        public int TotalHours { get; set; } = 2345;

        public void OnGet()
        {
            // Dans un cas réel, ces données viendraient de la base de données
        }
    }
}

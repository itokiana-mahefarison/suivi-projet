using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Backoffice.Config.Database;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System.Linq;

namespace Backoffice.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public int ActiveProjects { get; set; }
        public int TotalUsers { get; set; }
        public int TotalTasks { get; set; }
        public Double TotalHours { get; set; }
        public List<Project> RecentProjects { get; set; }
        public ProjectStatistics Statistics { get; set; }

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            // Récupération des statistiques de base
            ActiveProjects = await _context.Projects.CountAsync(p => p.Tasks.Any(t => t.Status != "done"));
            TotalUsers = await _context.Users.CountAsync();
            TotalTasks = await _context.Tasks.CountAsync();
            TotalHours = await _context.Tasks.Where(t => t.RealDuration.HasValue)
                                           .SumAsync(t => t.RealDuration.Value);

            // Récupération des projets récents (3 derniers)
            RecentProjects = await _context.Projects
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .Select(p => new Project
                {
                    Title = p.Title,
                    Tasks = p.Tasks,
                    // Calcul du pourcentage de progression
                    Progress = p.Tasks.Any() 
                        ? (double)p.Tasks.Count(t => t.Status == "done") / p.Tasks.Count * 100 
                        : 0
                })
                .ToListAsync();

            // Calcul des statistiques des projets
            Statistics = new ProjectStatistics
            {
                CompletedProjects = await _context.Projects.CountAsync(p => p.Tasks.All(t => t.Status == "done")),
                InProgressProjects = await _context.Projects.CountAsync(p => p.Tasks.Any(t => t.Status != "done")),
                TeamMembers = await _context.Users.CountAsync(),
                AverageProjectDuration = await CalculateAverageProjectDurationAsync()
            };
        }

        private async Task<int> CalculateAverageProjectDurationAsync()
        {
            var completedProjects = await _context.Projects
                .Include(p => p.Tasks)
                .Where(p => p.Tasks.Any() && p.Tasks.All(t => t.Status == "done"))
                .Select(p => new
                {
                    StartDate = p.Tasks.Min(t => t.StartDate),
                    EndDate = p.Tasks.Max(t => t.EndDate)
                })
                .ToListAsync();

            return completedProjects.Any()
                ? (int)completedProjects.Average(p => (p.EndDate - p.StartDate)?.Days)
                : 0;
        }
    }

    public class ProjectStatistics
    {
        public int CompletedProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int TeamMembers { get; set; }
        public int AverageProjectDuration { get; set; }
    }
}

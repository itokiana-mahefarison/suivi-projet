using CsvHelper;
using CsvHelper.Configuration;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Backoffice.Config.Database;
using Shared.Models;

namespace Backoffice.Pages.Projects
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DetailsModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public Project Project { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Project = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Project == null)
            {
                return NotFound();
            }

            Project.Progress = Project.Tasks.Any()
                ? (double)Project.Tasks.Count(t => t.Status == "done") / Project.Tasks.Count * 100
                : 0;

            return Page();
        }

        public async Task<IActionResult> OnPostImportTasksAsync(int projectId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrEmpty)
            };

            using var csv = new CsvReader(reader, config);
            var tasks = csv.GetRecords<TaskImportDto>().ToList();
            
            var taskEntities = tasks.Select(t => new Tasks
            {
                ProjectId = projectId,
                Title = t.Title,
                Description = t.Description,
                EstimatedDuration = t.EstimatedDuration,
                Status = t.Status
            }).ToList();

            await _context.Tasks.AddRangeAsync(taskEntities);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = projectId });
        }

        public async Task<IActionResult> OnPostGeneratePdfAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            var fileName = $"quote_{project.Title}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(_environment.WebRootPath, "temp", fileName);

            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "temp"));

            using (var writer = new PdfWriter(filePath))
            {
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                // Header with client info
                var clientInfo = new Paragraph()
                    .Add(project.Client.Name + "\n")
                    .Add(project.Client.Phone + "\n")
                    .Add(project.Client.Address + "\n");
                document.Add(clientInfo);

                // Project title
                document.Add(new Paragraph($"Quote: {project.Title}")
                    .SetFontSize(20));

                // Tasks table
                var table = new Table(4)
                    .UseAllAvailableWidth()
                    .SetMarginTop(20);

                table.AddHeaderCell("Task");
                table.AddHeaderCell("Description");
                table.AddHeaderCell("Duration (h)");
                table.AddHeaderCell("Cost");

                var totalCost = 0.0;

                foreach (var task in project.Tasks)
                {
                    table.AddCell(task.Title);
                    table.AddCell(task.Description ?? "");
                    table.AddCell((task.EstimatedDuration * 8)?.ToString() ?? "0");
                    
                    // Calcul du coût en fonction de l'utilisateur assigné
                    var cost = 0.0;
                    if (task.User != null && task.EstimatedDuration.HasValue)
                    {
                        cost = task.EstimatedDuration.Value * (task.User.HourlyRate ?? 0);
                    }
                    totalCost += cost;
                    table.AddCell($"${cost:F2}");
                }

                table.AddCell("Total");
                table.AddCell("");
                table.AddCell("");
                table.AddCell($"${totalCost:F2}");

                document.Add(table);

                // Footer
                document.Add(new Paragraph("This document was generated by Project Manager Platform")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(50));
            }

            var memory = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);

            return File(memory, "application/pdf", fileName);
        }
    }

    public class TaskImportDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public double? EstimatedDuration { get; set; }
        public string Status { get; set; }
    }
} 
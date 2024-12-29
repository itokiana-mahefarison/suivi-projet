using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Backoffice.Config.Database;

namespace Backoffice.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClientInputModel NewClient { get; set; }
        public List<Client> Clients { get; set; }

        public async Task OnGetAsync()
        {
            // Récupération des clients
            Clients = await _context.Clients.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var client = new Client
            {
                Name = NewClient.Name,
                Address = NewClient.Address,
                City = NewClient.City,
                Country = NewClient.Country,
                Phone = NewClient.Phone
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }

    public class ClientInputModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
    }
} 
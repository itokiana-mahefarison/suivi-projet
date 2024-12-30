using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Backoffice.Config.Database;
using Shared.Utils.Pagination;

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
        public Client NewClient { get; set; }
        public PaginatedList<Client> Clients { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty]
        public Client EditClient { get; set; }

        public async Task OnGetAsync()
        {
            if (!Request.Query.ContainsKey("pageNumber"))
                PageNumber = 1;
            if (!Request.Query.ContainsKey("pageSize"))
                PageSize = 10;

            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                SearchTerm = SearchTerm.Trim().ToLower();
                query = query.Where(c => 
                    c.Name.ToLower().Contains(SearchTerm) ||
                    (c.Address != null && c.Address.ToLower().Contains(SearchTerm)) ||
                    (c.City != null && c.City.ToLower().Contains(SearchTerm)) ||
                    (c.Country != null && c.Country.ToLower().Contains(SearchTerm)) ||
                    (c.Phone != null && c.Phone.ToLower().Contains(SearchTerm))
                );
            }

            query = query.Where(c => c.DeletedAt == null);
            Clients = await query.ToPagedListAsync(PageNumber, PageSize);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            _context.Clients.Add(NewClient);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { pageNumber = 1, pageSize = 10 });
        }

        public async Task<IActionResult> OnPostEditAsync(int id, string name, string address, string city, string country, string phone)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            client.Name = name;
            client.Address = address;
            client.City = city;
            client.Country = country;
            client.Phone = phone;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            client.DeletedAt = DateTime.UtcNow;
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
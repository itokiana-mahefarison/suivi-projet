using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class User: BaseEntity
    {
        public string? Name {  get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Address { get; set; }
        public string? Country {  get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public Role? Role { get; set; }
        public ICollection<Task>? Tasks { get; set; }
    }
}

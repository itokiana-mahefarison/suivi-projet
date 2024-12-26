using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Client: BaseEntity
    {
        public required string Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone {  get; set; }
        public ICollection<Project>? Projects { get; set; }

    }
}

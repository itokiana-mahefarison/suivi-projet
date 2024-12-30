using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Project: BaseEntity
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public Double? Budget { get; set; }
        public Client? Client { get; set; }
        public int? ClientId { get; set; }
        public ICollection<Tasks>? Tasks { get; set; }
        public Double? Progress { get; set; }

    }
}

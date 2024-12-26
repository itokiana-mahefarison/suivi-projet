using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Role: BaseEntity
    {
        public required string Label { get; set; }
        
        public ICollection<User>? Users { get; set; }

    }
}

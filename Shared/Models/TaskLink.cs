using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class TaskLink: BaseEntity
    {
        public int TaskFromId { get; set; }
        public Tasks TaskFrom {  get; set; }
        public int TaskToId { get; set;}
        public Tasks TaskTo { get; set; }
        public int LinkType { get; set;}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Task: BaseEntity
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Durée réelle de la tache exprimée en heures (h)
        /// </summary>
        public Double? RealDuration { get; set; }
        /// <summary>
        /// Durée estimée de la tache exprimée en heures (h)
        /// </summary>
        public Double? EstimatedDuration { get; set; }
        public User? User { get; set; }
        public required Project Project { get; set; }
    }
}

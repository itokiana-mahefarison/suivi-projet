using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Models
{
    public class Tasks: BaseEntity
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
        public string Status { get; set; } = "todo";
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public int? ParentTaskId { get; set; }
        public Tasks? ParentTask { get; set; }
        public ICollection<Tasks>? SubTasks { get; set;}
        public ICollection<TaskLink> TaskLinks { get; set; }
    }
}

using DailyTaskManager.Enums;
using System.ComponentModel.DataAnnotations;

namespace DailyTaskManager.DTOs
{
    public class UpdateTaskDto
    {
        [StringLength(45)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }

        public TaskPriority? Priority { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
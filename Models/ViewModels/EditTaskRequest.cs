using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPP.Models.ViewModels
{
    public class EditTaskRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}

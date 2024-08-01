using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPP.Models.ViewModels
{
    public class EditTaskRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        [Required(ErrorMessage = "Assigned to is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string AssignedTo { get; set; }
    }
}

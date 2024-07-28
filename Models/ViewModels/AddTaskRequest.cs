namespace TaskManagementAPP.Models.ViewModels
{
    public class AddTaskRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}

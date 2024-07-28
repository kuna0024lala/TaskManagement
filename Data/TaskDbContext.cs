using Microsoft.EntityFrameworkCore;
using TaskManagementAPP.Models.Domain;

namespace TaskManagementAPP.Data
{
    public class TaskDbContext: DbContext
    {
        public TaskDbContext(DbContextOptions options) : base(options) 
        {
            
        }

        public DbSet<TaskItem> TaskItems { get; set; }
    }
}

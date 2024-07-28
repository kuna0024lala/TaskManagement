using Microsoft.EntityFrameworkCore;
using TaskManagementAPP.Data;
using TaskManagementAPP.Models.Domain;
using TaskManagementAPP.Repositories.Interface;

namespace TaskManagementAPP.Repositories.Implementation
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext taskDbContext;

        public TaskRepository(TaskDbContext taskDbContext)
        {
            this.taskDbContext = taskDbContext;
        }


        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            await taskDbContext.AddAsync(task);
            await taskDbContext.SaveChangesAsync();
            return task;
        }

        public async Task<int> CountAsync()
        {
            return await taskDbContext.TaskItems.CountAsync();
        }

        public async Task<TaskItem?> DeleteAsync(int id)
        {
          var existingTask =  await taskDbContext.TaskItems.FindAsync(id);
          
          if (existingTask != null)
            {
                taskDbContext.TaskItems.Remove(existingTask);
                await taskDbContext.SaveChangesAsync();
                return existingTask;
            }

             return null;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync(
            string? searchQuery,
            string? sortBy,
            string? sortDirection,
            int pageNumber = 1,
            int pageSize =100)
        {
            var query = taskDbContext.TaskItems.AsQueryable();

            //Filtering
            if (string.IsNullOrWhiteSpace(searchQuery) == false)
            {
                query = query.Where(x => x.Title.Contains(searchQuery));
            }
           
            //sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                var isDesc = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase) ? true : false;

                if (string.Equals(sortBy, "Title", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title);
                };
                
            }

            //pagination
            var skipResult = (pageNumber -1) * pageSize;
            query = query.Skip(skipResult).Take(pageSize);

            return await query.ToListAsync();
         // return await taskDbContext.TaskItems.ToListAsync();
        }

       

        public async Task<TaskItem?> GetAsync(int id)
        {
          return  await taskDbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TaskItem?> UpdateAsync(TaskItem task)
        {
           var existingTask = await taskDbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == task.Id);

            if (existingTask != null)
            {
                existingTask.Id = task.Id;
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.DueDate = task.DueDate;
                existingTask.IsCompleted = task.IsCompleted;

                await taskDbContext.SaveChangesAsync();
                return existingTask;
            }

            return null;


        }
    }
}

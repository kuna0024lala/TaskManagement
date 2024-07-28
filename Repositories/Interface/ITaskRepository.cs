

using TaskManagementAPP.Models.Domain;

namespace TaskManagementAPP.Repositories.Interface
{
    public interface ITaskRepository
    {
       Task<IEnumerable<TaskItem>> GetAllAsync(
           string? searchQuery = null,
           string? sortBy = null,
           string? sortDirection = null,
           int pageNumber =1,
           int pageSize =100);

       Task<TaskItem?> GetAsync(int id);

       Task<TaskItem> AddAsync(TaskItem task);

       Task<TaskItem?> UpdateAsync(TaskItem task);

       Task<TaskItem?> DeleteAsync(int id);

       Task<int> CountAsync();

    }
}

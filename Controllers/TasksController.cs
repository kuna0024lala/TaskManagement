using Microsoft.AspNetCore.Mvc;
using TaskManagementAPP.Models.Domain;
using TaskManagementAPP.Models.ViewModels;
using TaskManagementAPP.Repositories.Interface;

namespace TaskManagementAPP.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskRepository taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTaskRequest addTaskRequest)
        {
            //Map View model to domain model
            var taskitem = new TaskItem
            {
                Title = addTaskRequest.Title,
                Description = addTaskRequest.Description,
                DueDate = addTaskRequest.DueDate,
                IsCompleted = addTaskRequest.IsCompleted,
            };


            await taskRepository.AddAsync(taskitem);

            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> List(
            string? searchQuery,
            string? sortBy,
            string? sortDirection,
            int pagesize = 3,
            int pageNumber = 1)
        {
            var totalRecords = await taskRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pagesize);

            if (pageNumber > totalPages)
            {
                pageNumber--;
            }
            if (pageNumber < 1)
            {
                pageNumber++;
            }

            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.PageSize = pagesize;
            ViewBag.PageNumber = pageNumber;
            //call the repository
            var taskitems = await taskRepository.GetAllAsync(searchQuery,sortBy,sortDirection,pageNumber,pagesize); 

            return View(taskitems);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieve the results from the repository
            var taskitem = await taskRepository.GetAsync(id);

            if (taskitem != null)
            {
                //map the domain model to view model
                var model = new EditTaskRequest
                {
                    Id = taskitem.Id,
                    Title = taskitem.Title,
                    Description = taskitem.Description,
                    DueDate = taskitem.DueDate,
                    IsCompleted = taskitem.IsCompleted,
                };
                return View(model); 
            }

           

            //pass data to view
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTaskRequest editTaskRequest)
        {
            //map view model to domain model
            var taskItemDomainModel = new TaskItem
            {
                Id = editTaskRequest.Id,
                Title = editTaskRequest.Title,
                Description = editTaskRequest.Description,
                DueDate = editTaskRequest.DueDate,
                IsCompleted = editTaskRequest.IsCompleted,
            };

            //submit information to repository to update 
           var updatedTask = await taskRepository.UpdateAsync(taskItemDomainModel);

            if (updatedTask != null)
            {
                //show success notification
                return RedirectToAction("Edit");
            }
            //show error notification 

            return RedirectToAction("Edit");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(EditTaskRequest editTaskRequest)
        {
            //Talk to repository to delete this task
           var deletedTask =  await taskRepository.DeleteAsync(editTaskRequest.Id);
            if (deletedTask != null)
            {
                //show success notification
                return RedirectToAction("List");
            }

            //show error notification 
            return RedirectToAction("Edit", new { id = editTaskRequest.Id});
        }

    }
}

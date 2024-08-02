using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
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
                    AssignedTo = addTaskRequest.AssignedTo,
                    IsCompleted = addTaskRequest.IsCompleted,
                };


                await taskRepository.AddAsync(taskitem);
                TempData["Message"] = "Task added successfully!";

                return RedirectToAction("List");
            
            
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
                    AssignedTo = taskitem.AssignedTo,
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
                    AssignedTo = editTaskRequest.AssignedTo,
                    IsCompleted = editTaskRequest.IsCompleted,
                };

                //submit information to repository to update 
                var updatedTask = await taskRepository.UpdateAsync(taskItemDomainModel);

                if (updatedTask != null)
                {
                    //show success notification
                    TempData["Message"] = "Task updated successfully!";
                    return RedirectToAction("List");
                }
            
            
            //show error notification 

            return RedirectToAction("Edit");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id )
        {
            //Talk to repository to delete this task
            var deletedTask =  await taskRepository.DeleteAsync(id);
            if (deletedTask != null)
            {
                //show success notification
                TempData["Message"] = "Task deleted successfully!";
                return RedirectToAction("List");
            }

            //show error notification 
           return RedirectToAction("Edit", new { id });
        }


        public async Task<IActionResult> ExportToExcel(string selectedIds)
        {
            if (string.IsNullOrEmpty(selectedIds))
            {
                TempData["Message"] = "No tasks selected for export.";
                return RedirectToAction("List");
            }

            var taskIds = selectedIds.Split(',').Select(int.Parse).ToList();
            var tasks = await taskRepository.GetAllAsync();

            var taskItemsList = tasks.Where(t => taskIds.Contains(t.Id)).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tasks");

                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Title";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "DueDate";
                worksheet.Cells[1, 5].Value = "AssignedTo";
                worksheet.Cells[1, 6].Value = "IsCompleted";

                for (int i = 0; i < taskItemsList.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = taskItemsList[i].Id;
                    worksheet.Cells[i + 2, 2].Value = taskItemsList[i].Title;
                    worksheet.Cells[i + 2, 3].Value = taskItemsList[i].Description;
                    worksheet.Cells[i + 2, 4].Value = taskItemsList[i].DueDate.HasValue ? taskItemsList[i].DueDate.Value.ToString("yyyy-MM-dd") : "N/A";
                    worksheet.Cells[i + 2, 5].Value = taskItemsList[i].AssignedTo;
                    worksheet.Cells[i + 2, 6].Value = taskItemsList[i].IsCompleted ? "Yes" : "No";
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Tasks_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                TempData["Message"] = "Tasks exported successfully!";
                return File(stream, contentType, fileName);
            }
        }

    }
}

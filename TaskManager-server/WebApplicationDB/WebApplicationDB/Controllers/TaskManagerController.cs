using Microsoft.AspNetCore.Mvc;
using WebApplicationDB.Models.Dto;
using WebApplicationDB.Models.Entity;
using WebApplicationDB.Models.Service;

namespace WebApplicationDB.Controllers 
{
    [ApiController]
    [Route("[controller]")]
    public class TaskManagerController : ControllerBase 
    {
        private readonly TaskManagerService _taskManager;

        private readonly ILogger<TaskManagerController> _logger;

        public TaskManagerController(TaskManagerService taskManager, ILogger <TaskManagerController> logger) 
        {
            _taskManager = taskManager;
            _logger = logger;
           
        }

        [HttpGet("get_tasks")]
        public IActionResult GetTaks(TasksStatus taskStatus, int page, int pageSize)
        {
            try
            {
                if (pageSize > 100) 
                {
                    _logger.LogInformation("The client trying to get more then 100 tasks.");

                    return BadRequest("We only approve up to 100 tasks in a single request.");
                }

                _logger.LogInformation($"Request for tasks of status: {taskStatus}");
                
                var (finishedTasks, totalCount) = _taskManager.GetTasks(taskStatus, page, pageSize);
                
                return Ok(new { Tasks = finishedTasks, TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("create_task")]
        public IActionResult CreateTask([FromBody] NewTaskDto newTaskDto) 
        {
            
            try 
            {
                if (newTaskDto == null) 
                {
                    _logger.LogInformation("Check json in the request create new task");
                    
                    return BadRequest("Failed to convert from json to dto");
                }

                _taskManager.addNewTask(newTaskDto);

                return Ok(new { Message = "Task created successfully", Task = newTaskDto });
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
            
        }
        
        [HttpPost]
        [Route("task_ended")]
        public IActionResult MarkTaskAsEnded(int taskId, bool isActive = false) 
        {
            
            bool check = _taskManager.taskEnded(taskId, isActive);
            if (check) 
            {
                return Ok(new { Message = "Task Ended Successfully"});
            }
            
            return StatusCode(500, "The Task Do Not Exist");
             
        }

        [HttpPost]
        [Route("delete_task")]
        public IActionResult DeleteTask(int id) 
        {
            bool check = _taskManager.DeleteTask(id);
            if (check) 
            {
                return Ok(new { Message = "Task Deleted Successfully"});
            }
            
            return StatusCode(500, "The Task Do Not Delete");

        }

        [HttpGet]
        [Route("get_finish_task")]
        public IActionResult GetAllFinishedTasks(int page, int pageSize) 
        {

            // Get all finished task that not in Archive 
            try 
            {
                var archiveTasks = _taskManager.GetAllFinishedTasks(page, pageSize);

                return Ok(archiveTasks);
            }
            catch (Exception ex) 
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("get_all_task_types")]
        public IActionResult GetAllTaskTypes()
        {
            try
            {
                var taskTypes = _taskManager.GetAllTaskTypes();
                return Ok(taskTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("move_task_toggle_archive")]
        public IActionResult ToggleTaskArchiveStatus(int id) 
        {

            bool check = _taskManager.ToggleTaskDateArchiveStatus(id);
            if (check) 
            {
                _logger.LogInformation("The task ID: " + id  + " Toggle archive Status");

                return Ok(new { Message = "The Task has been successfully Toggle"});
            }
            
            return StatusCode(500, "The Task was not found");

        }
    }
}

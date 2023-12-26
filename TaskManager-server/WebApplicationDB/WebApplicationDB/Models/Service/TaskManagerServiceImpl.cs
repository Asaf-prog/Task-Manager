using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApplicationDB.Controllers;
using WebApplicationDB.Data;
using WebApplicationDB.Models.Dto;
using WebApplicationDB.Models.Entity;


namespace WebApplicationDB.Models.Service 
{
     public class TaskManagerServiceImpl : TaskManagerService 
    {
        private readonly TaskContext _dbContext = new TaskContext();
        
        private readonly ILogger<TaskManagerController> _logger;

        private Timer timer;

        public TaskManagerServiceImpl (ILogger<TaskManagerController> logger) 
        {
            _logger = logger;
            ScheduleTaskCheck();
        }

        private void ScheduleTaskCheck() 
        {
            timer = new Timer(CheckAndMoveFinishedTasks, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            
            _logger.LogInformation("Begin the timer invocation.");
        }

        private void CheckAndMoveFinishedTasks(object state) 
        {
            try 
            {
                using (var dbContext = new TaskContext()) 
                {

                    DateTime oneWeekAgo = DateTime.Today.AddDays(-7);
                
                    var tasksToMove = dbContext.Tasks
                    ?.Include(t => t.StartDates)
                    ?.Where(t => t.StartDates
                            .Any(startDate =>  startDate.EndDateTime != null &&
                                            startDate.EndDateTime < oneWeekAgo &&
                                            !startDate.IsArchive))
                            .ToList();

                    if (tasksToMove != null) 
                    {

                        foreach (var task in tasksToMove ?? Enumerable.Empty<TaskEntity>()) 
                        {

                            if (task?.StartDates != null) 
                            {
                                foreach (var startDate in task.StartDates
                                    .Where(startDate =>
                                        startDate != null &&
                                        startDate.EndDateTime != null &&
                                        startDate.EndDateTime < oneWeekAgo &&
                                        !startDate.IsArchive))
                                {
                                    startDate.IsArchive = true;
                                }
                            }
                        }
                    }

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void addNewTask(NewTaskDto newTaskDto) 
        {
            // Check if a TaskEntity with the same properties already exists
            var task = _dbContext.Tasks
                ?.Include(t => t.StartDates)
                .FirstOrDefault(t =>
                    t.Name == newTaskDto.Name &&
                    t.Description == newTaskDto.Description &&
                    t.TaskType == newTaskDto.TaskType&&
                    t.IsRepeated == newTaskDto.IsRepeated);
            
            if (task != null) 
            {
                // TaskEntity already exists, add the new TaskDate to its StartDates
                if (task.StartDates == null) 
                {
                        task.StartDates = new List <TaskDate>();
                }
                task.StartDates.Add(new TaskDate 
                {
                    StartDateTime = newTaskDto.StartTime,
                    EndDateTime = null,
                    IsArchive = false
                });
            }
            else 
            {
                //add function that check if the taskType key exist in db
                TaskEntity newTask = new TaskEntity 
                {

                    Name = newTaskDto.Name,
                    Description = newTaskDto.Description,
                    TaskType = newTaskDto.TaskType ,
                    IsRepeated = newTaskDto.IsRepeated,
                    StartDates = new List<TaskDate> 
                    {
                            new TaskDate 
                            {
                                StartDateTime = newTaskDto.StartTime,
                                EndDateTime = null,
                                IsArchive = false
                            }
                    }
            };

                _dbContext.Tasks.Add(newTask);

            }
            _dbContext.SaveChanges();
        }

        public bool taskEnded(int taskId, bool isActive)
        {
            try
            {
                var existingTask = _dbContext.Tasks
                    .Include(t => t.StartDates)
                    .FirstOrDefault(t => t.StartDates.Any(date => date.Id == taskId));

                if (existingTask != null)
                {
                    var dateToUpdate = existingTask.StartDates
                           .FirstOrDefault(date =>
                                 date.Id == taskId );
                    if (dateToUpdate != null)
                    {
                        dateToUpdate.EndDateTime = DateTime.Today;
                        
                        // When a task is marked as ended, I check if the task is set to repeat.
                        // If the task is set to repeat,
                        // I create a new task with a start time that corresponds to the current time when the last task ended.
                        // If you wish to stop the repetition, you should delete the last task before completing them.

                        if(existingTask.IsRepeated && isActive )
                        {
                            addNewTask(ConvertTaskDtoToNewTaskDto(existingTask));
                        }

                        _dbContext.SaveChanges();
                        return true;
                    }
                }

                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public (IEnumerable<TaskDto> tasks, int count) GetTasks(TasksStatus taskStatus, int page, int pageSize) 
        {
            if (page < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page and pageSize must be greater than zero.");
            }

            var allTasks = _dbContext.Tasks
            .SelectMany(t => t.StartDates
                .Where(date =>
                    (taskStatus == TasksStatus.Active && date.IsArchive == false) ||
                    (taskStatus == TasksStatus.Archive && date.IsArchive == true) ||
                    taskStatus == TasksStatus.All
                )
                .OrderByDescending(startDate => startDate.StartDateTime)
                .Select(startDate => new TaskDto.Builder()
                    .AddName(t.Name)
                    .AddDescription(t.Description)
                    .AddTaskType(t.TaskType)
                    .AddIsRepeated(t.IsRepeated)
                    .AddStartTime(startDate.StartDateTime)
                    .AddEndTime(startDate.EndDateTime)
                    .AddIdDate(startDate.Id)
                    .AddIsArchive(startDate.IsArchive)
                    .Build())
            )
            .ToList();

            var totalCount = allTasks.Count();

            var orderedTasks = allTasks
                .OrderByDescending(taskDto => taskDto.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (orderedTasks,totalCount);
        }

        public bool DeleteTask(int taskId)
        {
            try
            {
                var taskToDelete = _dbContext.Tasks
                    .Include(t => t.StartDates)
                    .FirstOrDefault(t => t.StartDates.Any(date => date.Id == taskId));

                if (taskToDelete != null)
                {
                    if (taskToDelete.StartDates.Count == 1)
                    {
                        // If the task has only one date, delete the entire task
                        _dbContext.Tasks.Remove(taskToDelete);
                    }
                    else
                    {
                        // If the task has multiple dates, remove the specified date
                        var dateToDelete = taskToDelete.StartDates.FirstOrDefault(date => date.Id == taskId);
                        if (dateToDelete != null)
                        {
                            taskToDelete.StartDates.Remove(dateToDelete);
                        }
                    }

                    _dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        public IEnumerable<TaskDto> GetAllFinishedTasks(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page and pageSize must be greater than zero.");
            }

                var finishedTasks = _dbContext.Tasks
                .Where(t => t.StartDates.Any(date => date.EndDateTime != null))
                .SelectMany(t => t.StartDates
                    .Where(date => date.EndDateTime != null)
                    .Select(startDate => new TaskDto.Builder()
                        .AddName(t.Name)
                        .AddDescription(t.Description)
                        .AddTaskType(t.TaskType)
                        .AddIsRepeated(t.IsRepeated)
                        .AddStartTime(startDate.StartDateTime)
                        .AddEndTime(startDate.EndDateTime)
                        .AddIdDate(startDate.Id)
                        .AddIsArchive(startDate.IsArchive)
                        .Build()
                    )
                )
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return finishedTasks;
        }

        public IEnumerable<Tuple<int, string>> GetAllTaskTypes()
        {
            var taskTypes = _dbContext.TaskTypes
                .Select(tt => new Tuple<int, string>(tt.Id, tt.Type))
                .ToList();

            return taskTypes;
        }

        public bool ToggleTaskDateArchiveStatus(int dateId)
        {
            try
            {
                var dateToChange = _dbContext.TaskDates.FirstOrDefault(date => date.Id == dateId);

                if (dateToChange != null)
                {
                    // Toggle the IsArchive property
                    dateToChange.IsArchive = !dateToChange.IsArchive;

                    _dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private NewTaskDto ConvertTaskDtoToNewTaskDto (TaskEntity task)
        {
            NewTaskDto newTaskDto = new NewTaskDto(task.Name, task.Description, task.TaskType,task.IsRepeated, DateTime.Today);
            return newTaskDto;
        }
    }
}
    

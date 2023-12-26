using WebApplicationDB.Models.Dto;
using WebApplicationDB.Models.Entity;

namespace WebApplicationDB.Models.Service 
{

    public interface TaskManagerService 
    {
        public void addNewTask(NewTaskDto newTaskDto);
        public bool taskEnded(int taskId, bool isActive);
        public (IEnumerable<TaskDto> tasks, int count) GetTasks(TasksStatus taskStatus, int page, int pageSize);
        public bool DeleteTask(int taskId);
        public IEnumerable<TaskDto> GetAllFinishedTasks(int page, int pageSize);
        public bool ToggleTaskDateArchiveStatus(int dateId);
        public IEnumerable<Tuple<int, string>> GetAllTaskTypes();

    }
}

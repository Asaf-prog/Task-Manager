namespace WebApplicationDB.Models.Dto 
{

    public class TaskDto 
    {
        public TaskDto() {}

        public string Name { get; set; }
         public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public int TaskType { get; set; }
        public bool IsRepeated { get; set; }
        public bool IsArchive { get; set; }
         public int IdDate { get; set; }
       
        public class Builder 
        {
            private TaskDto task;

            public Builder() 
            {
                task = new TaskDto();
            }

            
            public Builder AddName(string name) 
            {
                task.Name = name;
                return this;
            }

            public Builder AddDescription(string? description) 
            {
                task.Description = description;
                return this;
            }

            public Builder AddTaskType(int taskType) 
            {
                task.TaskType = taskType;
                return this;
            }

            public Builder AddIsRepeated(bool isRepeated) 
            {
                task.IsRepeated = isRepeated;
                return this;
            }

            public Builder AddStartTime(DateTime startTime) 
            {
                task.StartTime = startTime;
                return this;
            }

           public Builder AddEndTime(DateTime? endTime) 
            {
                task.EndTime = endTime;
                return this;
            }

            public Builder AddIdDate(int idDate)
            {
                task.IdDate = idDate;
                return this;
            }

            public Builder AddIsArchive(bool isArchive) 
            {
                task.IsArchive = isArchive;
                return this;
            }

            public TaskDto Build() 
            {
                return task;
            }         
        }
    }
}

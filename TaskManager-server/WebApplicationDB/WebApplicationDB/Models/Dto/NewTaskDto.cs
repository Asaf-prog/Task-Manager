namespace WebApplicationDB.Models.Dto
{
    public class NewTaskDto 
    {
        public NewTaskDto() {}
        public NewTaskDto (string name, string? description, int taskType, bool isRepeated, DateTime startTime)
        {
            this.Name = name;
            this.Description = description;
            this.TaskType = taskType;
            this.IsRepeated = isRepeated;
            this.StartTime = startTime;
        }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int TaskType { get; set; }
        public bool IsRepeated { get; set; } 
        public DateTime StartTime { get; set; }       
    }
}
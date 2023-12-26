namespace WebApplicationDB.Models.Entity 
{

    public class TaskEntity 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int TaskType { get; set; }
        public TaskTypeTable TaskTypeTable {get; set;}
        public bool IsRepeated { get; set; }
        public ICollection<TaskDate> StartDates { get; set; }

    }
}

namespace WebApplicationDB.Models.Entity 
{
    
    public class TaskDate 
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsArchive { get; set; }
        
        // Foreign key to link to Task
        public int TaskEntityId { get; set; }
        public TaskEntity Task { get; set; }
    }
}

namespace WebApplicationDB.Models.Dto
{
    public class TaskClientDto 
    {
        public TaskClientDto() {}
        
        public string Name { get; set; }
        public string Description { get; set; }
        public int TaskType { get; set; }
        public bool IsRepeated { get; set; } 
        public bool? IsActive {get; set; }
        public int? IdOfDate { get; set; }
    }
}
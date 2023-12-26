namespace WebApplicationDB.Models.Entity 
{
    public class TaskTypeTable
    {
        public int Id { get; set; }
        public string Type { get; set; }

        public ICollection<TaskEntity> TaskEntities { get; set; }
    }
}

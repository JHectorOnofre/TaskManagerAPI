namespace TaskManagerAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsDeleted { get; set; } = false;
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>(); // regresa una lista vacía
    }
}

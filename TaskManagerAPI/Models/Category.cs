namespace TaskManagerAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }     // 26ene Código corto
        public bool IsActive { get; set; }   // 26ene Para futuros catálogos

        public bool IsDeleted { get; set; } = false;
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>(); // regresa una lista vacía
    }
}

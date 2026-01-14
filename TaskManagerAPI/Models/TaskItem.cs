namespace TaskManagerAPI.Models
{
    public class TaskItem
    {
        /* 
         * modelo relacionado con una tabla en BD llamada "tasks" 
         * 3 propiedades base: Id (obtener llave primaria), título e IsComplete
         * y un valor bool (para saber si está completa)
         */

        public int Id { get; set; } 
        public string Title { get; set; }
        public bool IsComplete { get; set; }

        public int Step { get; set;  } // gestionar el paso en el que va

        public DateTime CreatedAt { get; set; } = DateTime.Now; // gestionar cuándo se creó el registro con un valor por defecto 


        // 8enero
        public int? CategoryId { get; set; } = 0; // llave foránea relación con tabla Category
        public Category? Category { get; set; } // referencia en C# de hacia qué modelo apunta (relacion e tablas) a nivel código (no BD)
    }
}

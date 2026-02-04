namespace TaskManagerAPI.DTOs.Task
{
    public class ExcelTaskCatalog
    {
        // 3feb | 0? Estructura intermedia para coincidir columnas nuevo excel
        public string Title { get; set; } // Columna B del Excel
        public bool IsCompleted { get; set; } // Column C
        public int CategoryId { get; set; } // Column E
    }
}

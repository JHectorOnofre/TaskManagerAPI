namespace TaskManagerAPI.DTOs.Task
{
    public class TaskSearchResult
    {
        // probar cambiar el nombre de las variables para diferenciar:
        public int Identificador { get; set; }
        public string TituloLibro { get; set; }
        public bool Completada { get; set; }

        public int PasoActual { get; set; } // gestionar el paso en el que va
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        
    }
}

namespace TaskManagerAPI.DTOs.Task
{
    public class UpdateTaskRequest
    {
        public string Title { get; set; }
        public bool? IsCompleted { get; set; } // ? Habilita valor null

        // solo tiene sentido que se especifique el paso
        //public int step { get; set; }

    }

}

using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.Task
{
    public class UpdateTaskRequest
    {
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Error: El título solo permite letras y números.")]
        public string Title { get; set; }
        public bool? IsCompleted { get; set; } // ? Habilita valor null

        // solo tiene sentido que se especifique el paso
        //public int step { get; set; }

    }

}

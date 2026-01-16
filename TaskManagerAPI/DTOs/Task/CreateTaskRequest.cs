using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.Task
{
    // se recibe cuando alguien trate de guardar un registro
    public class CreateTaskRequest
    {
        // extiende a todos los lugares donde se usa CreateTaskRequest
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "El título puede contener entre 2 y 200 letras.")]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = "CategoryId es requerido.")] 
        public int CategoryId { get; set; }

    }
}

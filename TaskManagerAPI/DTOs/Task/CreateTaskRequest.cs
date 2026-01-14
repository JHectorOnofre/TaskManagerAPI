using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.Task
{
    // se recibe cuando alguien trate de guardar un registro
    public class CreateTaskRequest
    {
        // extiende a todos los lugares donde se usa CreateTaskRequest
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = "CategoryId es requerido.")] 
        public int CategoryId { get; set; }

    }
}

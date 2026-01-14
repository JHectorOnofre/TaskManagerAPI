using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.Category
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
        public string Name { get; set; }
    }
}

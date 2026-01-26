using TaskManagerAPI.DTOs.Category;

namespace TaskManagerAPI.Interfaces.Categories
{
    public interface ICategoryService
    {
        /* 1) se actualiza la Interfaz con los métodos que vienen del Controlador
         * 
         * (Donde se definen los contrados)
         */
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(); // GET/categories
        Task<CategoryDto?> GetCategoryByIdAsync(int id); // GET /categories{id}
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request); //POST categories{id}

        Task<int> ImportCategoriesFromExcelAsync(IFormFile file); // 26 ene: Para migración lógica servicio de Excel
    }
}

using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.Task;

namespace TaskManagerAPI.Interfaces.Tasks
{
    public interface ITaskService
    {
        /* 13 ene
         * Interfaz de servicio o contrato: capa inter entre controlador y servicio
         * - Define qué va a hacer el servicio (cuáles son los métodos disponibles, pero NO el cómo) 
         * - 
         * AdvancedSearhAsync = búsquedas asíncronas en bd
         */
        Task<PagedResultDto<TaskWithCategoryDto>> AdvancedSearchAsync(
            string? text,
            bool? completed,
            int? step,
            int? categoryId,
            string? categoryName,
            int page,
            int pageSize
        );

        //1. Actualizando la Interfaz con los métodos que provienen de TaskItemsController.cs

        Task<IEnumerable<TaskItemResponse>> GetTasksAsync(); // GET /tasks
        Task<TaskItemResponse?> GetByIdAsync(int id); // GET {id:int}
        Task<TaskItemResponse> CreateTaskAsync(CreateTaskRequest request); // POST/tasks
        Task<bool> UpdateTaskAsync(int id, UpdateTaskRequest taskDto); // PUT {id}
        Task<bool> DeleteTaskAsync(int id); // Delete {id}
        Task<IEnumerable<TaskSearchResult>> SearchAsync(TaskSearchRequest request); // GET Search

        Task<IEnumerable<TaskSearchResult>> GetPagedTasksAsync(int page, int pageSize); // GET "paged"
        
        Task<IEnumerable<TaskWithCategoryDto>> GetTasksWithCategoryAsync(); // para servicio GET "with category"


    } // Scope de la interfaz

}

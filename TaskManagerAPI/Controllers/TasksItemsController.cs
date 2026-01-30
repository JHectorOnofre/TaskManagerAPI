using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.Task;
using TaskManagerAPI.Interfaces.Tasks;
using TaskManagerAPI.Models;
using TaskManagerAPI.Utilities.Exceptions;



namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaksItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITaskService _taskService; //13 enero

        public TaksItemsController(AppDbContext context, ITaskService taskService)
        {
            _context = context;
            _taskService = taskService;
        }



        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> Get() // se agrega async, ActionResult
        {
            var result = await _taskService.GetTasksAsync(); // llamada al servicio, se guarda en var result

            return Ok(result);
        } 



        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskItemResponse>> GetById(int id)
        {
            var result = await _taskService.GetByIdAsync(id); // se llama al resultado de lo que hace  servicio (TaskService)

            if (result == null) // revisa si el servicio devuelve algo, si no: 404 
                return NotFound();

            return Ok(result); // entrega el resultado final dado por el servicio TaskService
        }



        [HttpPost]
        public async Task<ActionResult<TaskItemResponse>> Create([FromBody] CreateTaskRequest request) 
        {
            if (request == null)
                return BadRequest("Body requerido.");

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title es requerido.");

            var dto = await _taskService.CreateTaskAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto); // devuelve la respuesta exitosa con la ruta 
        }



        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
        {
            if (request == null) return BadRequest("Body requerido.");
            if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest("Title es requerido.");

            var result = await _taskService.UpdateTaskAsync(id, request);

            if (!result) return NotFound();

            return NoContent(); // 204
        }



        [HttpDelete("{id:int}")] // Se debe hacer un borrado lógico que se mantiene en memoria (campo deleted)
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id); //llamada al servicio (TaskSercice.cs)

            if (!result) return NotFound(); // si el servicio da "false", se devuelve como antes "NotFound"

            return NoContent(); // si todo sale bien, el "NoContent" que se tenía antes
        }



        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TaskSearchResult>>> Search([FromQuery] TaskSearchRequest request)
        {

            var results = await _taskService.SearchAsync(request); //obtener sólo el resultado sin la lógica de por medio (el cómo)

            return Ok(results);
        }



        [HttpGet("paged")] // método que responde peticiones Get en la ruta nombrada como "paged"
        public async Task<ActionResult<IEnumerable<TaskSearchResult>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)

        {
            // Llama al método del servicio pasando los parámetros que recibimos por URL
            var result = await _taskService.GetPagedTasksAsync(page, pageSize);

            return Ok(result);
        }



        [HttpGet("with-category")]
        public async Task<ActionResult<IEnumerable<TaskWithCategoryDto>>> GetWithCategory()
        {

            var result = await _taskService.GetTasksWithCategoryAsync(); // en lugar de usar _context ahora es _taskService
            return Ok(result);
        } 



        [HttpGet("advanced-search")]
        public async Task<ActionResult<PagedResultDto<TaskWithCategoryDto>>> AdvancedSearch (
            [FromQuery] string? text,
            [FromQuery] bool? completed,
            [FromQuery] int? step,
            [FromQuery] int? categoryId,
            [FromQuery] string? categoryName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {

            //throw new Exception("La categoría no existe."); //14 ene | timeout error 21enero

            //13 enero:
            if (page <= 0) return BadRequest("Page debe ser mayor a 0.");
            if (pageSize <= 0 || pageSize > 100) return BadRequest("PageSize debe estar entre 1 y 100.");

            var result = await _taskService.AdvancedSearchAsync(
                text, completed, step, categoryId, categoryName, page, pageSize);

            return Ok(result);
        }
    }

}



/* DTO = Data Transfer Option
 * para transferir datos entre capas o sistemas
 * - retorna lo que se pide, en lugar de exponer la entidad directamente
 * - permite cambiar la BD sin romper la estructura
 * - así como los ID en BD suelen ser campos autoincrementados, no es algo que un usuario haga
 * 
 * !Los DTO viven en el backend
*/
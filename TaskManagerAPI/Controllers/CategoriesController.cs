using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.DTOs.Category;
using TaskManagerAPI.Interfaces.Categories;
using TaskManagerAPI.Interfaces.Tasks;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")] // <- significa que la base es api/categories
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        //private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
                _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Get()
        {
            // Solo llamamos al servicio, la lógica está en CategoryService.cs
            var result = await _categoryService.GetCategoriesAsync();
            return Ok(result);
        }


        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request)
        {
            // El servicio nos devuelve ya el DTO creado.
            var dto = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            // Preguntamos al servicio. Si devuelve null, el controlador decide mandar NotFound.
            var dto = await _categoryService.GetCategoryByIdAsync(id);
            if (dto == null) return NotFound();

            return Ok(dto);
        }


        [HttpPost("import-excel")] // 26ene: Siempre método POST para cargar archivos
        public async Task<IActionResult> ImportFromExcel(IFormFile file) //ImportFromExcel = librería para recibir el excel
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se recibió ningún archivo o está vacío.");

            // Llamamos al servicio y él se encarga de todo el trabajo con la lógica
            var count = await _categoryService.ImportCategoriesFromExcelAsync(file);

            return Ok(new { Message = $"Se importaron {count} categorías nuevas." });
        }

    }

}

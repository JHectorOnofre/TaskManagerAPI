using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.DTOs.Category;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriesController (AppDbContext context) // En AppDbContext se tienen las tablas accesibles en este contexto
        {
            _context = context; 
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Get()
        {
            var result = await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(result);
        }


        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request)
        {
            // Si usas [ApiController], ModelState se valida automáticamente.
            // Igual puedes explicar que si falla devuelve 400.

            var entity = new Category
            {
                Name = request.Name.Trim()
            };

            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();

            var dto = new CategoryDto
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }


        [HttpGet("{id:int}")] //9 enero: obtener categoría en específico
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var entity = await _context.Categories.FindAsync(id);
            if (entity == null) return NotFound();

            var dto = new CategoryDto
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return Ok(dto);
        }
    }

}

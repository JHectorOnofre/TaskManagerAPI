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
    [Route("api/[controller]")]
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


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CategoryDto>>> Get()
        //{
        //    var result = await _context.Categories
        //        .OrderBy(c => c.Name)
        //        .Select(c => new CategoryDto
        //        {
        //            Id = c.Id,
        //            Name = c.Name
        //        })
        //        .ToListAsync();

        //return Ok(result);
        //}


        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request)
        {
            // El servicio nos devuelve ya el DTO creado.
            var dto = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }


        //[HttpPost]
        //public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request)
        //{

        //    var entity = new Category
        //    {
        //        Name = request.Name.Trim()
        //    };

        //    _context.Categories.Add(entity);
        //    await _context.SaveChangesAsync();

        //    var dto = new CategoryDto
        //    {
        //        Id = entity.Id,
        //        Name = entity.Name
        //    };

        //    return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        //}

        

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            // Preguntamos al servicio. Si devuelve null, el controlador decide mandar NotFound.
            var dto = await _categoryService.GetCategoryByIdAsync(id);
            if (dto == null) return NotFound();

            return Ok(dto);
        }


        //[HttpPost("import-excel")] // 26ene: Siempre método POST para cargar archivos
        //public async Task<IActionResult> ImportFromExcel(IFormFile file) //ImportFromExcel = librería para recibir el excel
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No se recibió ningún archivo o está vacío.");

        //    var categories = new List<Category>();

        //    using (var stream = new MemoryStream()) // Crear archivo virtual en la RAM
        //    {
        //        await file.CopyToAsync(stream);
        //        stream.Position = 0; // Nos aseguramos de ir al inicio del archivo

        //        using (var workbook = new XLWorkbook(stream)) //XLM librería para abrir el archivo copiado en la memoria virtual y moverse a través
        //        {
        //            var worksheet = workbook.Worksheets.First(); // Tomamos la primera hoja
        //            var rows = worksheet.RangeUsed().RowsUsed(); // conteo de cuántas casillas del archivo se están usando para iterar sobre ellas

        //            bool isHeader = true; // para excluir la primera fila (ya que en este caso se usa como cabecera)

        //            foreach (var row in rows) //recorrer las casillas en uso 
        //            {
        //                if (isHeader)
        //                {
        //                    // Saltar la fila de cabeceras
        //                    isHeader = false;
        //                    continue;
        //                }

        //                // 3 líneas para extraer los datos de cada columna (de izq-derecha)
        //                var name = row.Cell(1).GetString();      // Columna A
        //                var code = row.Cell(2).GetString();      // Columna B
        //                var isActiveCell = row.Cell(3).GetString(); // Columna C

        //                bool isActive = true;
        //                if (!string.IsNullOrWhiteSpace(isActiveCell))
        //                {
        //                    // TRUE/FALSE, 1/0, Sí/No... aquí se puede refinar
        //                    bool.TryParse(isActiveCell, out isActive);
        //                }

        //                if (string.IsNullOrWhiteSpace(name))
        //                {
        //                    // Se puede decidir saltar o romper
        //                    continue;
        //                }

        //                var category = new Category //lista vacía de la entidad categorías
        //                {
        //                    Name = name.Trim(),
        //                    Code = code?.Trim(),
        //                    IsActive = isActive
        //                };

        //                categories.Add(category);
        //            }
        //        }
        //    }

        //    // Validaciones 
        //    // Opcional: filtrar duplicados por Name en la misma importación
        //    categories = categories
        //        .GroupBy(c => c.Name.ToLower())
        //        .Select(g => g.First())
        //        .ToList();

        //    // Opcional: evitar insertar categorías que ya existan en la BD
        //    var existingNames = _context.Categories
        //        .Select(c => c.Name.ToLower())
        //        .ToHashSet(); // Para rendimiento: Es un símil de trabajar en un Array con un índice (Trabaja de forma instantánea sin recorrer todo buscando)

        //    var newCategories = categories
        //        .Where(c => !existingNames.Contains(c.Name.ToLower()))
        //        .ToList();

        //    // Guardar en base de datos
        //    _context.Categories.AddRange(newCategories);
        //    await _context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        Message = $"Se importaron {categories.Count} categorías."
        //    });
        //}
    }

}

using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.Category;
//using TaskManagerAPI.DTOs.Category; // Cambiar a category
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Interfaces.Categories;
// using TaskManagerAPI.Interfaces.Categories; // category*
using TaskManagerAPI.Models;
using TaskManagerAPI.Utilities.Exceptions;

namespace TaskManagerAPI.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        // 2) Migrar la lógica
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        //GET
        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }


        // Método CREATE
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var entity = new Category { 
                Name = request.Name.Trim(),
            Code = request.Name.Length >=3
            ? request.Name.Substring(0,8).ToUpper()
            : "Category"
            };

            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();

            return new CategoryDto { Id = entity.Id, Name = entity.Name };
        }

        // Get by {id}
        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var entity = await _context.Categories.FindAsync(id);
            if (entity == null) return null; // Devolvemos null para que el controlador sepa que no hubo resultado

            return new CategoryDto { Id = entity.Id, Name = entity.Name };
        }


        // 26 enero: lógica proveniente del controlador al servicio (service layer) para (POST EXCEL)
        // Dentro de CategoryService.cs
        public async Task<int> ImportCategoriesFromExcelAsync(IFormFile file)
        {
            var categories = new List<Category>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.First();
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip(1) reemplaza el bool isHeader

                    foreach (var row in rows)
                    {
                        var name = row.Cell(1).GetString();
                        if (string.IsNullOrWhiteSpace(name)) continue;

                        var category = new Category
                        {
                            Name = name.Trim(),
                            Code = row.Cell(2).GetString()?.Trim(),
                            IsActive = bool.TryParse(row.Cell(3).GetString(), out var active) ? active : true
                        };
                        categories.Add(category);
                    }
                }
            }

            // Filtrar duplicados y guardar (Aquí el _context sí funciona)
            var uniqueCategories = categories.GroupBy(c => c.Name.ToLower()).Select(g => g.First()).ToList();
            var existingNames = _context.Categories.Select(c => c.Name.ToLower()).ToHashSet();
            var newCategories = uniqueCategories.Where(c => !existingNames.Contains(c.Name.ToLower())).ToList();

            _context.Categories.AddRange(newCategories);
            await _context.SaveChangesAsync();

            return newCategories.Count;
        }
    }
}

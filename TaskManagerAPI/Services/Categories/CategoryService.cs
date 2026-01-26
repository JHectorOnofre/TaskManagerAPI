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
    }
}

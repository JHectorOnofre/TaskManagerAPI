/* 13 ene: Se agrega capa */

using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.Task;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Interfaces.Tasks;

public class TaskService : ITaskService
{

    private readonly AppDbContext _context;


    public TaskService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<PagedResultDto<TaskWithCategoryDto>> AdvancedSearchAsync(
        string? text,
        bool? completed,
        int? step,
        int? categoryId,
        string? categoryName,
        int page,
        int pageSize
        )
    {
        var query = _context.Tasks
            .Include(t => t.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(text))
            query = query.Where(t => t.Title.Contains(text));

        if (completed.HasValue)
            query = query.Where(t => t.IsComplete == completed);

        if (step.HasValue)
            query = query.Where(t => t.Step == step);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            var name = categoryName.Trim();
            query = query.Where(t => t.Category.Name.Contains(name));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskWithCategoryDto
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsComplete,
                Step = t.Step,
                CreatedAt = t.CreatedAt,
                CategoryId = t.CategoryId ?? 0,
                CategoryName = t.Category.Name
            })
            .ToListAsync();

        return new PagedResultDto<TaskWithCategoryDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }


    public async Task<IEnumerable<TaskWithCategoryDto>> GetTasksWithCategoryAsync()
    {
        // Lógica de GET "with category":
        var result = await _context.Tasks
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted) //15 ene: filtro que trae todos los registros donde valor isDeleted = false
            .OrderBy(t => t.Id)
            .Select(t => new TaskWithCategoryDto
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsComplete,
                Step = t.Step,
                CreatedAt = t.CreatedAt,
                CategoryId = t.CategoryId ?? 0,
                CategoryName = t.Category.Name
            })
            .ToListAsync();

        return result; 
    }


    public async Task<IEnumerable<TaskSearchResult>> GetPagedTasksAsync(int page, int pageSize)
    {
        var result = await _context.Tasks
            .OrderBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskSearchResult // lógica del query es: ordena, salta, toma
            {
                Identificador = t.Id,
                TituloLibro = t.Title,
                Completada = t.IsComplete,
                PasoActual = t.Step,
                FechaCreacion = t.CreatedAt
            })
            .ToListAsync();

        return result;
    }


    public async Task<IEnumerable<TaskSearchResult>> SearchAsync(TaskSearchRequest request)
    {
        var query = _context.Tasks.AsQueryable();

        // Filtros
        if (!string.IsNullOrWhiteSpace(request.text))
            query = query.Where(t => t.Title.Contains(request.text));

        if (request.completed.HasValue)
            query = query.Where(t => t.IsComplete == request.completed);

        if (request.step.HasValue)
            query = query.Where(t => t.Step == request.step);

        // Ordenamiento con Switch
        query = request.orderBy switch
        {
            "title" => query.OrderBy(t => t.Title),
            "title_desc" => query.OrderByDescending(t => t.Title),
            "date" => query.OrderBy(t => t.CreatedAt),
            "date_desc" => query.OrderByDescending(t => t.CreatedAt),
            "step" => query.OrderBy(t => t.Step),
            "step_desc" => query.OrderByDescending(t => t.Step),
            _ => query.OrderBy(t => t.Id) // _ para pasar valores vacíos
        };


        // Paginación y Proyección al DTO
        var results = await query
            .Skip((request.page - 1) * request.pageSize)
            .Take(request.pageSize)
            .Select(t => new TaskSearchResult
            {
                Identificador = t.Id,
                TituloLibro = t.Title,
                Completada = t.IsComplete,
                PasoActual = t.Step,
                FechaCreacion = t.CreatedAt
            })
            .ToListAsync();

        return results;
    }


}
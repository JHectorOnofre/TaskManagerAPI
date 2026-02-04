/* 13 ene: Se agrega capa */

using ClosedXML.Excel; //recibir / abrir? el archivo
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.Task;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Interfaces.Tasks;
using TaskManagerAPI.Models;
using TaskManagerAPI.Utilities.Exceptions;

public class TaskService : ITaskService // Servicio : Interfaz (puente de comunicación)
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


    public async Task<IEnumerable<TaskSearchResult>> SearchAsync(TaskSearchRequest request) // GET /search
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


    public async Task<bool> DeleteTaskAsync(int id) // DELETE{id}
    {
        var task = await _context.Tasks.FindAsync(id); // lógica de búsqueda
        if (task == null) return false; // lógica de validación

        _context.Tasks.Remove(task); // lóg de borrado: si existe, elimina la fila  
        await _context.SaveChangesAsync(); // lóg de persistencia: se aplican los cambios hacia la BD

        return true; // éxito
    }


    public async Task<bool> UpdateTaskAsync(int id, UpdateTaskRequest request) // PUT 
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            task.Title = request.Title.Trim();
        }

        if (request.IsCompleted.HasValue)
        {
            task.IsComplete = request.IsCompleted.Value;
        }

        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<TaskItemResponse> CreateTaskAsync(CreateTaskRequest request) // POST
    {
        // 1. Regla de negocio = validar la categoría dado un Id
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new BusinessException("La categoría no existe.", 404);


        var entity = new TaskItem // 2. Mapeo del Request a la Entidad
        {
            Title = request.Title.Trim(),
            IsComplete = false,
            CategoryId = request.CategoryId,
        };

        // 3. Persistencia
        _context.Tasks.Add(entity);
        await _context.SaveChangesAsync();

        // 4. Mapeo de Entidad a Response (DTO)
        return new TaskItemResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            IsCompleted = entity.IsComplete
        };
    }


    public async Task<TaskItemResponse?> GetByIdAsync(int id) // GET{id}
    {
        // CORTADO: La lógica de búsqueda en la BD
        var task = await _context.Tasks.FindAsync(id);

        // Si es nulo, devolvemos null (el controlador decidirá qué error HTTP enviar)
        if (task == null) return null;

        // CORTADO: El mapeo de la entidad al DTO
        return new TaskItemResponse
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsComplete
        };
    }


    public async Task<IEnumerable<TaskItemResponse>> GetTasksAsync()
    {
        var tasks = await _context.Tasks
            .Select(t => new TaskItemResponse
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsComplete
            })
        .ToListAsync();

        return (tasks);
    }



    //public async Task<int> ImportTasksFromExcelAsync(IFormFile file)
    //{
    //    var tasks = new List<TaskItem>(); // Del modelo que es TaskItem

    //    using (var stream = new MemoryStream())
    //    {
    //        await file.CopyToAsync(stream);
    //        stream.Position = 0;

    //        using (var workbook = new XLWorkbook(stream))
    //        {
    //            var worksheet = workbook.Worksheets.First();

    //            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Saltamos la primera fila (encabezados = 1)

    //            foreach (var row in rows) // El mapeo para recorrrer columnas A a la  F
    //            {

    //                var title = row.Cell(2).GetString();
    //                if (string.IsNullOrWhiteSpace(title)) continue;

    //                var task = new TaskItem
    //                {
    //                    Title = title.Trim(), // como title
    //                    IsComplete = row.Cell(3).GetBoolean(), // existe Boolean
    //                    Step = int.TryParse(row.Cell(4).GetString(), out var s) ? s : 0,
    //                    CategoryId = int.TryParse(row.Cell(5).GetString(), out var cId) ? cId : 0,
    //                    IsDeleted = false, // Por defecto al importar
    //                    CreatedAt = DateTime.Now // Si tu modelo tiene fecha de creación
    //                };

    //                tasks.Add(task);
    //            }
    //        }
    //    }

    //    if (tasks.Any())
    //    {
    //        _context.Tasks.AddRange(tasks);
    //        await _context.SaveChangesAsync();
    //    }
    //    return tasks.Count;
    //}



    // SIN DTO:
    public async Task<int> ImportTasksFromExcelAsync(IFormFile file)
    {
        var tasks = new List<TaskItem>();

        var existeCategoryId = await _context.Categories.Select(c => c.Id).ToHashSetAsync();


        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheets.First();
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // misma lógica abreviada para saltar la  1ra fila (cabecera)

                foreach (var row in rows)
                {
                    var title = row.Cell(2).GetString(); // Donde "title" (column B) es equivalente a name de la 1ra hoja de excel
                    if (string.IsNullOrWhiteSpace(title)) continue;

                    int cId = int.TryParse(row.Cell(5).GetString(), out var tempId) ? tempId : 0; //1. ternario para leer el category Id

                    if (!existeCategoryId.Contains(cId)) continue; // validar si la cat no existe (se salta)

                    string completeText = row.Cell(3).GetString().ToLower().Trim();
                    bool isCompleted = completeText == "TRUE" || completeText == "VERDADERO"; // leer y convertir el valor 

                    int step = int.TryParse(row.Cell(4).GetString(), out var s) ? s : 0;
                    
                    var task = new TaskItem
                    {
                        Title = title.Trim(),
                        IsComplete = isCompleted, // ya validado
                        Step = step,  //3 ya validado 
                        CategoryId = cId, // ya validado
                        IsDeleted = false, //row.Cell(6).GetBoolean(),
                        // craetedAt sería DateTime.Now?

                    };

                    tasks.Add(task);
                }

            }

        }
        if (tasks.Any()) // para guardar
        {
            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();
        }
        return tasks.Count;

    } //scope public



} // scope clase 
using Microsoft.EntityFrameworkCore;
using TaskManager.Utilities.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Para REGISTRAR el AppDbContext 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

builder.Services.AddServices(); //13ene: se incluye lo que esté en el método de extensión creado


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseMiddleware<GlobalErrorHandlerMiddleware>(); //14 ene: debe estar antes del mapeo de controladores (.MapController)

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

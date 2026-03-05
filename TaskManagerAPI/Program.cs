using Microsoft.EntityFrameworkCore;
using TaskManager.Utilities.Configurations;

var builder = WebApplication.CreateBuilder(args);

// parte de la corrección del 5 de febrero para la lectura de la url del API en el MVC: config la política de CORS (permite que el MVC consulte) 
builder.Services.AddCors(options => {
options.AddPolicy("AllowWebApp", policy => {
policy.WithOrigins("https://localhost:7137") // URL exacta de tu MVC
      .AllowAnyMethod()
      .AllowAnyHeader();
    }); // <--- Cierre de la política
}); // <--- Cierre del AddCors

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Para REGISTRAR el AppDbContext 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

builder.Services.AddServices(); //13ene: se incluye lo que esté en el método de extensión creado (ServiceConfiguration.cs)

var app = builder.Build();

// --- SECCIÓN DEL MIDDLEWARE
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseMiddleware<GlobalErrorHandlerMiddleware>(); //14 ene: debe estar antes del mapeo de controladores (.MapController)

app.UseHttpsRedirection();


app.UseCors("AllowWebApp"); // 2. ACTIVAR CORS (Debe ir antes de Authorization y MapControllers)
app.UseAuthorization();

app.MapControllers();

app.Run();

// 13 ene: se especifica en un archivo a parte la especificación para no saturar el "Program.cs" (es decir, es un método de extensión, segmentando las llamadas que se hacen desde el program.cs)
using TaskManagerAPI.Interfaces.Categories;
using TaskManagerAPI.Interfaces.Tasks;
using TaskManagerAPI.Services.Categories;

namespace TaskManager.Utilities.Configurations
{
    public static class ServiceConfiguration
    {

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskService, TaskService>(); // conecta la interfaz con la clase

            // 16 ene: se registra la nueva Interfaz x Clase para que la ID sepa qué entregar al controlador
            
            services.AddScoped<ICategoryService, CategoryService>(); // para servicio de categorías
        }
    }
}

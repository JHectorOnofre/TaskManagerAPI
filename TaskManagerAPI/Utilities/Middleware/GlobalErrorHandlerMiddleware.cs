using System.Net;
using System.Text.Json;
using TaskManagerAPI.Utilities.Exceptions;

public class GlobalErrorHandlerMiddleware
{
    private readonly RequestDelegate _next; // representa cuál és el sig. paso de la ejecución, otro middleware o el controlador donde esté la ejecución
    private readonly ILogger<GlobalErrorHandlerMiddleware> _logger; // permite registrar errores, guardando el detalle técnico de qué pasó

    public GlobalErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }


    public async Task InvokeAsync(HttpContext context) //se ejecuta con cada petición HTTP, si hay un error va a entrar aquí también para diagnóstico posterior
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }


    private static Task HandleExceptionAsync( // clase que permite capturar excepciones
        HttpContext context,
        Exception exception) // con los parámetros se gestiona como se responde a la exceción
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception is BusinessException be 
        ? be.StatusCode
        : StatusCodes.Status500InternalServerError; //15 ene: distinguir entre una excepción personalizada y las demás (op. ternario)

        context.Response.StatusCode = statusCode;

        var response = new // declaración de un objeto anónimo 
        {
            message = "Ocurrió un error inesperado.", // lo que se manda al lado del usuario
            detail = exception.Message // Traza técnica de cuál fue el error (rastreable para diagnóstico)
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
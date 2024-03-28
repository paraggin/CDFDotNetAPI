using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace CDF_WebApi.Middlewares
{
    public class GlobalExeptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExeptionHandlingMiddleware> _logger;
        public GlobalExeptionHandlingMiddleware(ILogger<GlobalExeptionHandlingMiddleware> logger) =>
            _logger = logger;


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = "Server error",
                    Title = "Server error",
                    Detail = "An internal server has occurred"
                };

                string json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
                context.Response.ContentType = "application/json";
            }
        }
    }
}

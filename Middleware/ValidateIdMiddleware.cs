using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SmartParkingBackend.Middleware
{
    public class ValidateIdMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Check if it's a GET request and contains an ID parameter
            if (context.Request.Method == "GET" && path != null)
            {
                var segments = path.Split('/');
                if (segments.Length >= 2)
                {
                    var lastSegment = segments[segments.Length - 1];
                    if (int.TryParse(lastSegment, out int id) && id <= 0)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("El ID debe ser mayor a 0");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
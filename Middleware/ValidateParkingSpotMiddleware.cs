using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SmartParkingBackend.Middleware
{
    public class ValidateParkingSpotMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateParkingSpotMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path != null && path.Contains("/rows/") && path.Contains("/spots/"))
            {
                var segments = path.Split('/');

                // Validate parkingId
                if (segments.Length >= 2)
                {
                    if (!int.TryParse(segments[1], out int parkingId) || parkingId <= 0)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("El parkingId debe ser un número mayor a 0");
                        return;
                    }
                }

                // Validate rowCode
                if (segments.Length >= 4)
                {
                    var rowCode = segments[3];
                    if (string.IsNullOrEmpty(rowCode) || !char.IsLetter(rowCode[0]))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("El rowCode debe ser una letra");
                        return;
                    }
                }

                // Validate spotCode
                if (segments.Length >= 6)
                {
                    if (!int.TryParse(segments[5], out int spotCode) || spotCode <= 0)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("El spotCode debe ser un número mayor a 0");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SmartParkingBackend.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConcurrentDictionary<string, DateTime> _lastRequestTimes = new();
        private const int RateLimitSeconds = 5;

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Solo aplicamos el rate limit a las rutas que actualizan el estado de los spots
            if (context.Request.Method == "PUT" && path != null && path.Contains("/spots/"))
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var key = $"{clientIp}:{path}";

                if (_lastRequestTimes.TryGetValue(key, out DateTime lastRequest))
                {
                    var timeSinceLastRequest = DateTime.UtcNow - lastRequest;
                    if (timeSinceLastRequest.TotalSeconds < RateLimitSeconds)
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsync($"Por favor espere {RateLimitSeconds} segundos entre actualizaciones");
                        return;
                    }
                }

                _lastRequestTimes.AddOrUpdate(key, DateTime.UtcNow, (_, __) => DateTime.UtcNow);
            }

            await _next(context);
        }
    }
}
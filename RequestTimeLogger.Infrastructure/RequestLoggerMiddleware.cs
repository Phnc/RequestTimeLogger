using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NLog;
using System.Diagnostics;

namespace RequestTimeLogger.Infraestructure.Middleware
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggerMiddleware> _logger;
        private static readonly Logger NLogger = LogManager.GetCurrentClassLogger();

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            try
            {
                _logger.LogInformation($"Received request: {request.Method} {request.Path}");

                await _next(context);

                _logger.LogInformation($"Response sent for {request.Method} {request.Path}");

                stopwatch.Stop();

                _logger.LogInformation($"Request completed in {stopwatch.ElapsedMilliseconds} ms");

                NLogger.Info(new
                {
                    RequestPath = request.Path,
                    RequestMethod = request.Method,
                    ResponseStatusCode = context.Response.StatusCode,
                    ElapsedTimeMs = stopwatch.ElapsedMilliseconds
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during request processing");
                throw;
            }
        }
    }
}

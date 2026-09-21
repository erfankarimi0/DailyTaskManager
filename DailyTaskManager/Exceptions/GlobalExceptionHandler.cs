using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskManager.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // خطای واقعی برای خود Developer در لاگ ثبت می‌شود
            _logger.LogError(exception, "Database or server error occurred.");

            httpContext.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = 500,
                    Title = "خطای سرور",
                    Detail = "خطایی هنگام پردازش درخواست رخ داده است."
                },
                cancellationToken);

            return true;
        }
    }
}
using Wolverine.Middleware;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Wolverine;
namespace  SharedKernel;

public class LoggingMiddleware<T>
{
    private readonly ILogger<LoggingMiddleware<T>> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware<T>> logger)
    {
        _logger = logger;
    }

    // Middleware signature in Wolverine:
    // (context, message, next)
    public async Task InvokeAsync(IMessageContext context, T message, Func<Task> next)
    {
        string messageName = typeof(T).Name;

        _logger.LogInformation("Processing  {Command}", messageName);

        try
        {
            await next(); // Execute the next middleware/handler

            _logger.LogInformation("Completed  {Command}", messageName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Completed  {Command} with error", messageName);
            throw new InvalidOperationException($"Error processing  {messageName}", ex);
        }
    }
}


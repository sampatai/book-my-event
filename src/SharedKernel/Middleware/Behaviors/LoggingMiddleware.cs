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

    // Executed before the handler
    public ValueTask BeforeAsync(T message, IMessageContext context)
    {
        _logger.LogInformation("Starting processing of {Command}", typeof(T).Name);
        return ValueTask.CompletedTask;
    }

    // Executed after the handler successfully completes
    public ValueTask AfterAsync(T message, IMessageContext context)
    {
        _logger.LogInformation("Successfully processed {Command}", typeof(T).Name);
        return ValueTask.CompletedTask;
    }

    // Executed after everything (success or exception)
    public ValueTask FinallyAsync(T message, IMessageContext context, Exception? ex)
    {
        if (ex != null)
        {
            _logger.LogError(ex, "Processing of {Command} failed", typeof(T).Name);
        }
        return ValueTask.CompletedTask;
    }
}



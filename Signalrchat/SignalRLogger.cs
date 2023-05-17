using Serilog;
using Serilog.Events;

namespace Signalrchat;

public class SignalRLogger : Microsoft.Extensions.Logging.ILogger
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    public SignalRLogger()
    {
        _logger = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog();
        }).CreateLogger("SignalRLogger");
    }

    #region Implementation of ILogger

    public IDisposable BeginScope<TState>(TState state)
    {
        return null; // If you need scope management, return a disposable scope object
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true; // Serilog handles log level filtering
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);

        // Log the message using the ILogger instance
        _logger.Log(logLevel, eventId, state, exception, formatter);
    }

    #endregion

    private LogEventLevel ConvertLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}
using Microsoft.AspNetCore.SignalR;

namespace Signalrchat;

public class SignalRLoggerProvider : ILoggerProvider
{
    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
    {
        if (categoryName == typeof(HubConnectionContext).FullName)
        {
            return new SignalRLogger();
        }

        return null;
    }

    public void Dispose()
    {
        // Cleanup if necessary
    }
}
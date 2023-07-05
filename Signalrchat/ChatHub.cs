using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Signalrchat;

[EnableCors("SignalRPolicy")]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string user, string message)
    {
        _logger.LogInformation("Received message from {User}: {Message}", user, message);

        // Process the message and send it to clients
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async IAsyncEnumerable<DateTime> Stream()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return DateTime.Now;
            await Task.Delay(1000);
        }
    }
}
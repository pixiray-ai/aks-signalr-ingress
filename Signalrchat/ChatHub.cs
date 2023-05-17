using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Signalrchat;

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
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FederationPlatform.Web.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task SendNotificationToUser(string userId, string title, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", title, message);
    }

    public async Task SendNotificationToAll(string title, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", title, message);
    }

    public async Task SendNotificationToAdmins(string title, string message)
    {
        await Clients.Group("Admins").SendAsync("ReceiveNotification", title, message);
    }

    public async Task SendNotificationToRepresentatives(string title, string message)
    {
        await Clients.Group("Representatives").SendAsync("ReceiveNotification", title, message);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var userRole = Context.User?.FindFirst("Role")?.Value;

        if (userRole == "Admin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
        else if (userRole == "Representative")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Representatives");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userRole = Context.User?.FindFirst("Role")?.Value;

        if (userRole == "Admin")
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }
        else if (userRole == "Representative")
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Representatives");
        }

        await base.OnDisconnectedAsync(exception);
    }
}

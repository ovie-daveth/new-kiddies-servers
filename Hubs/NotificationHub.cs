using ChatApp.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.Backend.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly INotificationService _notificationService;
    private readonly IConnectionManager _connectionManager;

    public NotificationHub(
        INotificationService notificationService,
        IConnectionManager connectionManager)
    {
        _notificationService = notificationService;
        _connectionManager = connectionManager;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        await _connectionManager.AddConnection(userId, Context.ConnectionId);
        
        // Send unread notifications count
        var unreadCount = await _notificationService.GetUnreadNotificationsCount(userId);
        await Clients.Caller.SendAsync("UnreadNotificationsCount", unreadCount);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        await _connectionManager.RemoveConnection(userId, Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkNotificationAsRead(int notificationId)
    {
        var userId = GetUserId();
        await _notificationService.MarkAsRead(userId, notificationId);
        
        var unreadCount = await _notificationService.GetUnreadNotificationsCount(userId);
        await Clients.Caller.SendAsync("UnreadNotificationsCount", unreadCount);
    }

    public async Task MarkAllAsRead()
    {
        var userId = GetUserId();
        await _notificationService.MarkAllAsRead(userId);
        await Clients.Caller.SendAsync("UnreadNotificationsCount", 0);
    }

    private int GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}


using ChatApp.Backend.DTOs;
using ChatApp.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.Backend.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly IConnectionManager _connectionManager;
    private readonly INotificationService _notificationService;

    public ChatHub(
        IChatService chatService,
        IConnectionManager connectionManager,
        INotificationService notificationService)
    {
        _chatService = chatService;
        _connectionManager = connectionManager;
        _notificationService = notificationService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        await _connectionManager.AddConnection(userId, Context.ConnectionId);
        await _chatService.SetUserOnlineStatus(userId, true);
        
        // Notify other users that this user is online
        await Clients.Others.SendAsync("UserOnline", userId);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        await _connectionManager.RemoveConnection(userId, Context.ConnectionId);
        
        // Check if user has other connections
        var connections = await _connectionManager.GetConnections(userId);
        if (!connections.Any())
        {
            await _chatService.SetUserOnlineStatus(userId, false);
            await Clients.Others.SendAsync("UserOffline", userId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(SendMessageDto messageDto)
    {
        var userId = GetUserId();
        var message = await _chatService.SendMessage(userId, messageDto);
        
        // Get all participants in the conversation
        var participants = await _chatService.GetConversationParticipants(messageDto.ConversationId);
        
        // Send message to all participants except sender
        foreach (var participantId in participants.Where(p => p != userId))
        {
            var connections = await _connectionManager.GetConnections(participantId);
            if (connections.Any())
            {
                await Clients.Clients(connections).SendAsync("ReceiveMessage", message);
            }
            
            // Create notification for offline users or users not in the conversation view
            await _notificationService.CreateMessageNotification(participantId, message);
        }
        
        // Confirm to sender
        await Clients.Caller.SendAsync("MessageSent", message);
    }

    public async Task JoinConversation(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        await Clients.Group($"conversation_{conversationId}").SendAsync("UserJoinedConversation", GetUserId(), conversationId);
    }

    public async Task LeaveConversation(int conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        await Clients.Group($"conversation_{conversationId}").SendAsync("UserLeftConversation", GetUserId(), conversationId);
    }

    public async Task TypingIndicator(int conversationId, bool isTyping)
    {
        var userId = GetUserId();
        await Clients.OthersInGroup($"conversation_{conversationId}")
                     .SendAsync("UserTyping", userId, conversationId, isTyping);
    }

    public async Task MarkMessageAsRead(int messageId)
    {
        var userId = GetUserId();
        await _chatService.MarkMessageAsRead(userId, messageId);
        
        // Notify sender that message was read
        var message = await _chatService.GetMessage(messageId);
        if (message != null)
        {
            var senderConnections = await _connectionManager.GetConnections(message.SenderId);
            await Clients.Clients(senderConnections).SendAsync("MessageRead", messageId, userId);
        }
    }

    private int GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}


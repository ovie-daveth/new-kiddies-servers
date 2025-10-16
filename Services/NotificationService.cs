using ChatApp.Backend.Data;
using ChatApp.Backend.DTOs;
using ChatApp.Backend.Hubs;
using ChatApp.Backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Backend.Services;

public class NotificationService : INotificationService
{
    private readonly ChatDbContext _context;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IConnectionManager _connectionManager;

    public NotificationService(
        ChatDbContext context,
        IHubContext<NotificationHub> notificationHub,
        IConnectionManager connectionManager)
    {
        _context = context;
        _notificationHub = notificationHub;
        _connectionManager = connectionManager;
    }

    public async Task<NotificationDto> CreateNotification(CreateNotificationDto notificationDto)
    {
        var notification = new Notification
        {
            UserId = notificationDto.UserId,
            ActorUserId = notificationDto.ActorUserId,
            Title = notificationDto.Title,
            Message = notificationDto.Message,
            Type = notificationDto.Type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            Data = notificationDto.Data
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Get actor details if available
        User? actor = null;
        if (notificationDto.ActorUserId.HasValue)
        {
            actor = await _context.Users.FindAsync(notificationDto.ActorUserId.Value);
        }

        var notificationResult = new NotificationDto
        {
            Id = notification.Id,
            ActorUserId = notification.ActorUserId,
            ActorUsername = actor?.Username,
            ActorDisplayName = actor?.DisplayName,
            ActorProfilePictureUrl = actor?.ProfilePictureUrl,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            Data = notification.Data
        };

        // Send notification via SignalR
        await SendNotificationToUser(notificationDto.UserId, notificationResult);

        return notificationResult;
    }

    public async Task CreateMessageNotification(int userId, MessageDto message)
    {
        var notificationDto = new CreateNotificationDto
        {
            UserId = userId,
            Title = "New Message",
            Message = $"{message.SenderDisplayName ?? message.SenderUsername}: {message.Content}",
            Type = NotificationType.NewMessage,
            Data = System.Text.Json.JsonSerializer.Serialize(new { message.ConversationId, message.Id })
        };

        await CreateNotification(notificationDto);
    }

    public async Task CreateCommentNotification(int userId, int postId, CommentDto comment)
    {
        // Don't notify yourself
        if (userId == comment.UserId)
            return;

        var notificationDto = new CreateNotificationDto
        {
            UserId = userId,  // Post owner receives notification
            ActorUserId = comment.UserId,  // Commenter is the actor
            Title = "New Comment",
            Message = $"{comment.DisplayName ?? comment.Username} commented on your post: \"{comment.Content}\"",
            Type = NotificationType.PostComment,
            Data = System.Text.Json.JsonSerializer.Serialize(new { PostId = postId, CommentId = comment.Id })
        };

        await CreateNotification(notificationDto);
    }

    public async Task CreateReplyNotification(int userId, int postId, CommentDto reply)
    {
        // Don't notify yourself
        if (userId == reply.UserId)
            return;

        var notificationDto = new CreateNotificationDto
        {
            UserId = userId,  // Original commenter receives notification
            ActorUserId = reply.UserId,  // Replier is the actor
            Title = "New Reply",
            Message = $"{reply.DisplayName ?? reply.Username} replied to your comment: \"{reply.Content}\"",
            Type = NotificationType.CommentReply,
            Data = System.Text.Json.JsonSerializer.Serialize(new { PostId = postId, CommentId = reply.Id, ParentCommentId = reply.ParentCommentId })
        };

        await CreateNotification(notificationDto);
    }

    public async Task CreatePostLikeNotification(int postOwnerId, int postId, int likerUserId)
    {
        // Don't notify yourself
        if (postOwnerId == likerUserId)
            return;

        // Get the liker's info
        var liker = await _context.Users.FindAsync(likerUserId);
        if (liker == null)
            return;

        var notificationDto = new CreateNotificationDto
        {
            UserId = postOwnerId,  // Post owner receives notification
            ActorUserId = likerUserId,  // Liker is the actor
            Title = "Post Liked",
            Message = $"{liker.DisplayName ?? liker.Username} liked your post!",
            Type = NotificationType.PostLike,
            Data = System.Text.Json.JsonSerializer.Serialize(new { PostId = postId, LikerUserId = likerUserId })
        };

        await CreateNotification(notificationDto);
    }

    public async Task CreateCommentLikeNotification(int commentOwnerId, int commentId, int likerUserId)
    {
        // Don't notify yourself
        if (commentOwnerId == likerUserId)
            return;

        // Get the liker's info
        var liker = await _context.Users.FindAsync(likerUserId);
        if (liker == null)
            return;

        var notificationDto = new CreateNotificationDto
        {
            UserId = commentOwnerId,  // Comment owner receives notification
            ActorUserId = likerUserId,  // Liker is the actor
            Title = "Comment Liked",
            Message = $"{liker.DisplayName ?? liker.Username} liked your comment!",
            Type = NotificationType.CommentLike,
            Data = System.Text.Json.JsonSerializer.Serialize(new { CommentId = commentId, LikerUserId = likerUserId })
        };

        await CreateNotification(notificationDto);
    }

    public async Task CreateFriendRequestNotification(int addresseeId, int requesterId)
    {
        // Get the requester's info
        var requester = await _context.Users.FindAsync(requesterId);
        if (requester == null)
            return;

        var notificationDto = new CreateNotificationDto
        {
            UserId = addresseeId,  // User receiving the friend request
            ActorUserId = requesterId,  // User sending the friend request
            Title = "Friend Request",
            Message = $"{requester.DisplayName ?? requester.Username} sent you a friend request!",
            Type = NotificationType.FriendRequest,
            Data = System.Text.Json.JsonSerializer.Serialize(new { RequesterId = requesterId })
        };

        await CreateNotification(notificationDto);
    }

    public async Task CreateFriendRequestAcceptedNotification(int requesterId, int accepterId)
    {
        // Get the accepter's info
        var accepter = await _context.Users.FindAsync(accepterId);
        if (accepter == null)
            return;

        var notificationDto = new CreateNotificationDto
        {
            UserId = requesterId,  // Original requester receives notification
            ActorUserId = accepterId,  // User who accepted
            Title = "Friend Request Accepted",
            Message = $"{accepter.DisplayName ?? accepter.Username} accepted your friend request!",
            Type = NotificationType.FriendRequestAccepted,
            Data = System.Text.Json.JsonSerializer.Serialize(new { UserId = accepterId })
        };

        await CreateNotification(notificationDto);
    }

    public async Task CreateNewFollowerNotification(int followingId, int followerId)
    {
        // Don't notify yourself
        if (followingId == followerId)
            return;

        // Get the follower's info
        var follower = await _context.Users.FindAsync(followerId);
        if (follower == null)
            return;

        var notificationDto = new CreateNotificationDto
        {
            UserId = followingId,  // User being followed receives notification
            ActorUserId = followerId,  // User following
            Title = "New Follower",
            Message = $"{follower.DisplayName ?? follower.Username} started following you!",
            Type = NotificationType.NewFollower,
            Data = System.Text.Json.JsonSerializer.Serialize(new { FollowerId = followerId })
        };

        await CreateNotification(notificationDto);
    }

    public async Task<List<NotificationDto>> GetUserNotifications(int userId, int skip = 0, int take = 20)
    {
        var notifications = await _context.Notifications
            .Include(n => n.Actor)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                ActorUserId = n.ActorUserId,
                ActorUsername = n.Actor != null ? n.Actor.Username : null,
                ActorDisplayName = n.Actor != null ? n.Actor.DisplayName : null,
                ActorProfilePictureUrl = n.Actor != null ? n.Actor.ProfilePictureUrl : null,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                Data = n.Data
            })
            .ToListAsync();

        return notifications;
    }

    public async Task<int> GetUnreadNotificationsCount(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task MarkAsRead(int userId, int notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsRead(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SendNotificationToUser(int userId, NotificationDto notification)
    {
        var connections = await _connectionManager.GetConnections(userId);
        if (connections.Any())
        {
            await _notificationHub.Clients.Clients(connections).SendAsync("ReceiveNotification", notification);
        }
    }
}


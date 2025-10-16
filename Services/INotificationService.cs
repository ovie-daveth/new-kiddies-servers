using ChatApp.Backend.DTOs;

namespace ChatApp.Backend.Services;

public interface INotificationService
{
    Task<NotificationDto> CreateNotification(CreateNotificationDto notificationDto);
    Task CreateMessageNotification(int userId, MessageDto message);
    Task CreateCommentNotification(int userId, int postId, CommentDto comment);
    Task CreateReplyNotification(int userId, int postId, CommentDto reply);
    Task CreatePostLikeNotification(int postOwnerId, int postId, int likerUserId);
    Task CreateCommentLikeNotification(int commentOwnerId, int commentId, int likerUserId);
    Task CreateFriendRequestNotification(int addresseeId, int requesterId);
    Task CreateFriendRequestAcceptedNotification(int requesterId, int accepterId);
    Task CreateNewFollowerNotification(int followingId, int followerId);
    Task<List<NotificationDto>> GetUserNotifications(int userId, int skip = 0, int take = 20);
    Task<int> GetUnreadNotificationsCount(int userId);
    Task MarkAsRead(int userId, int notificationId);
    Task MarkAllAsRead(int userId);
    Task SendNotificationToUser(int userId, NotificationDto notification);
}


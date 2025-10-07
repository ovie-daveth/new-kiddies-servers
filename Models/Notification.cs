namespace ChatApp.Backend.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }  // Who receives the notification
    public int? ActorUserId { get; set; }  // Who performed the action
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Data { get; set; } // JSON data for additional context
    
    // Navigation properties
    public User User { get; set; } = null!;
    public User? Actor { get; set; }  // The user who triggered the notification
}

public enum NotificationType
{
    NewMessage,
    PostComment,
    CommentReply,
    PostLike,
    CommentLike,
    MentionedInMessage,
    AddedToGroup,
    FriendRequest,
    System
}


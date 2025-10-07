using ChatApp.Backend.Models;

namespace ChatApp.Backend.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public int? ActorUserId { get; set; }
    public string? ActorUsername { get; set; }
    public string? ActorDisplayName { get; set; }
    public string? ActorProfilePictureUrl { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Data { get; set; }
}

public class CreateNotificationDto
{
    public int UserId { get; set; }  // Who receives it
    public int? ActorUserId { get; set; }  // Who did the action
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? Data { get; set; }
}


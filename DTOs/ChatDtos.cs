using ChatApp.Backend.Models;

namespace ChatApp.Backend.DTOs;

public class SendMessageDto
{
    public int ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;
}

public class MessageDto
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; } = string.Empty;
    public string? SenderDisplayName { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
}

public class CreateConversationDto
{
    public List<int> ParticipantIds { get; set; } = new();
    public string? Name { get; set; } // For group chats
    public bool IsGroup { get; set; }
}

public class ConversationDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsGroup { get; set; }
    public DateTime CreatedAt { get; set; }
    public MessageDto? LastMessage { get; set; }
    public List<UserDto> Participants { get; set; } = new();
    public int UnreadCount { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}


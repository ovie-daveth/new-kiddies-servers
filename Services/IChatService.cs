using ChatApp.Backend.DTOs;

namespace ChatApp.Backend.Services;

public interface IChatService
{
    Task<MessageDto> SendMessage(int senderId, SendMessageDto messageDto);
    Task<MessageDto?> GetMessage(int messageId);
    Task<List<MessageDto>> GetConversationMessages(int conversationId, int userId, int skip = 0, int take = 50);
    Task<ConversationDto> CreateConversation(int creatorId, CreateConversationDto conversationDto);
    Task<List<ConversationDto>> GetUserConversations(int userId);
    Task<ConversationDto?> GetConversation(int conversationId, int userId);
    Task<List<int>> GetConversationParticipants(int conversationId);
    Task SetUserOnlineStatus(int userId, bool isOnline);
    Task MarkMessageAsRead(int userId, int messageId);
}


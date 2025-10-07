using ChatApp.Backend.Data;
using ChatApp.Backend.DTOs;
using ChatApp.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Backend.Services;

public class ChatService : IChatService
{
    private readonly ChatDbContext _context;

    public ChatService(ChatDbContext context)
    {
        _context = context;
    }

    public async Task<MessageDto> SendMessage(int senderId, SendMessageDto messageDto)
    {
        // Verify user is participant
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == messageDto.ConversationId && cp.UserId == senderId);

        if (!isParticipant)
        {
            throw new UnauthorizedAccessException("User is not a participant of this conversation");
        }

        var message = new Message
        {
            ConversationId = messageDto.ConversationId,
            SenderId = senderId,
            Content = messageDto.Content,
            Type = messageDto.Type,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);

        // Update conversation last message time
        var conversation = await _context.Conversations.FindAsync(messageDto.ConversationId);
        if (conversation != null)
        {
            conversation.LastMessageAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // Load sender info
        var sender = await _context.Users.FindAsync(senderId);

        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderUsername = sender?.Username ?? "",
            SenderDisplayName = sender?.DisplayName,
            Content = message.Content,
            Type = message.Type,
            SentAt = message.SentAt,
            IsEdited = message.IsEdited,
            IsDeleted = message.IsDeleted
        };
    }

    public async Task<MessageDto?> GetMessage(int messageId)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null) return null;

        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderUsername = message.Sender.Username,
            SenderDisplayName = message.Sender.DisplayName,
            Content = message.Content,
            Type = message.Type,
            SentAt = message.SentAt,
            IsEdited = message.IsEdited,
            IsDeleted = message.IsDeleted
        };
    }

    public async Task<List<MessageDto>> GetConversationMessages(int conversationId, int userId, int skip = 0, int take = 50)
    {
        // Verify user is participant
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

        if (!isParticipant)
        {
            throw new UnauthorizedAccessException("User is not a participant of this conversation");
        }

        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                SenderUsername = m.Sender.Username,
                SenderDisplayName = m.Sender.DisplayName,
                Content = m.Content,
                Type = m.Type,
                SentAt = m.SentAt,
                IsEdited = m.IsEdited,
                IsDeleted = m.IsDeleted
            })
            .ToListAsync();

        messages.Reverse(); // Return in ascending order
        return messages;
    }

    public async Task<ConversationDto> CreateConversation(int creatorId, CreateConversationDto conversationDto)
    {
        // Add creator to participants if not included
        if (!conversationDto.ParticipantIds.Contains(creatorId))
        {
            conversationDto.ParticipantIds.Add(creatorId);
        }

        // For direct messages, check if conversation already exists
        if (!conversationDto.IsGroup && conversationDto.ParticipantIds.Count == 2)
        {
            var existingConversation = await _context.Conversations
                .Where(c => !c.IsGroup && c.Participants.Count == 2)
                .Where(c => c.Participants.All(p => conversationDto.ParticipantIds.Contains(p.UserId)))
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync();

            if (existingConversation != null)
            {
                return MapToConversationDto(existingConversation, creatorId);
            }
        }

        var conversation = new Conversation
        {
            Name = conversationDto.Name,
            IsGroup = conversationDto.IsGroup,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        foreach (var participantId in conversationDto.ParticipantIds)
        {
            var participant = new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = participantId,
                JoinedAt = DateTime.UtcNow
            };
            _context.ConversationParticipants.Add(participant);
        }

        await _context.SaveChangesAsync();

        // Reload with participants
        var createdConversation = await _context.Conversations
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .FirstAsync(c => c.Id == conversation.Id);

        return MapToConversationDto(createdConversation, creatorId);
    }

    public async Task<List<ConversationDto>> GetUserConversations(int userId)
    {
        var conversations = await _context.Conversations
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .ThenInclude(m => m.Sender)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .ToListAsync();

        return conversations.Select(c => MapToConversationDto(c, userId)).ToList();
    }

    public async Task<ConversationDto?> GetConversation(int conversationId, int userId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.Participants.Any(p => p.UserId == userId));

        if (conversation == null) return null;

        return MapToConversationDto(conversation, userId);
    }

    public async Task<List<int>> GetConversationParticipants(int conversationId)
    {
        return await _context.ConversationParticipants
            .Where(cp => cp.ConversationId == conversationId)
            .Select(cp => cp.UserId)
            .ToListAsync();
    }

    public async Task SetUserOnlineStatus(int userId, bool isOnline)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsOnline = isOnline;
            if (!isOnline)
            {
                user.LastSeen = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkMessageAsRead(int userId, int messageId)
    {
        var message = await _context.Messages
            .Include(m => m.Conversation)
                .ThenInclude(c => c.Participants)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null) return;

        var participant = message.Conversation.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant != null)
        {
            participant.LastReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private ConversationDto MapToConversationDto(Conversation conversation, int currentUserId)
    {
        var lastMessage = conversation.Messages.FirstOrDefault();
        
        // Calculate unread count
        var participant = conversation.Participants.FirstOrDefault(p => p.UserId == currentUserId);
        var unreadCount = 0;
        if (participant != null)
        {
            unreadCount = conversation.Messages
                .Count(m => m.SentAt > (participant.LastReadAt ?? DateTime.MinValue) && m.SenderId != currentUserId);
        }

        return new ConversationDto
        {
            Id = conversation.Id,
            Name = conversation.Name,
            IsGroup = conversation.IsGroup,
            CreatedAt = conversation.CreatedAt,
            LastMessage = lastMessage != null ? new MessageDto
            {
                Id = lastMessage.Id,
                ConversationId = lastMessage.ConversationId,
                SenderId = lastMessage.SenderId,
                SenderUsername = lastMessage.Sender.Username,
                SenderDisplayName = lastMessage.Sender.DisplayName,
                Content = lastMessage.Content,
                Type = lastMessage.Type,
                SentAt = lastMessage.SentAt,
                IsEdited = lastMessage.IsEdited,
                IsDeleted = lastMessage.IsDeleted
            } : null,
            Participants = conversation.Participants.Select(p => new UserDto
            {
                Id = p.User.Id,
                Username = p.User.Username,
                DisplayName = p.User.DisplayName,
                ProfilePictureUrl = p.User.ProfilePictureUrl,
                IsOnline = p.User.IsOnline,
                LastSeen = p.User.LastSeen
            }).ToList(),
            UnreadCount = unreadCount
        };
    }
}


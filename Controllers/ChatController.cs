using ChatApp.Backend.DTOs;
using ChatApp.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("conversations")]
    public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] CreateConversationDto conversationDto)
    {
        var userId = GetUserId();
        try
        {
            var conversation = await _chatService.CreateConversation(userId, conversationDto);
            return Ok(conversation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDto>>> GetConversations()
    {
        var userId = GetUserId();
        var conversations = await _chatService.GetUserConversations(userId);
        return Ok(conversations);
    }

    [HttpGet("conversations/{conversationId}")]
    public async Task<ActionResult<ConversationDto>> GetConversation(int conversationId)
    {
        var userId = GetUserId();
        var conversation = await _chatService.GetConversation(conversationId, userId);
        
        if (conversation == null)
        {
            return NotFound();
        }

        return Ok(conversation);
    }

    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
        int conversationId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var userId = GetUserId();
        try
        {
            var messages = await _chatService.GetConversationMessages(conversationId, userId, skip, take);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("messages")]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto messageDto)
    {
        var userId = GetUserId();
        try
        {
            var message = await _chatService.SendMessage(userId, messageDto);
            return Ok(message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}


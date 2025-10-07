using ChatApp.Backend.DTOs;
using ChatApp.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.Backend.Hubs;

[Authorize]
public class PostHub : Hub
{
    private readonly IPostService _postService;
    private readonly IConnectionManager _connectionManager;
    private readonly INotificationService _notificationService;

    public PostHub(
        IPostService postService,
        IConnectionManager connectionManager,
        INotificationService notificationService)
    {
        _postService = postService;
        _connectionManager = connectionManager;
        _notificationService = notificationService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        await _connectionManager.AddConnection(userId, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        await _connectionManager.RemoveConnection(userId, Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinPost(int postId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"post_{postId}");
    }

    public async Task LeavePost(int postId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"post_{postId}");
    }

    public async Task SendComment(CreateCommentDto commentDto)
    {
        var userId = GetUserId();
        var comment = await _postService.AddComment(userId, commentDto);
        
        // Broadcast comment to all users watching this post
        await Clients.Group($"post_{commentDto.PostId}").SendAsync("ReceiveComment", comment);
        
        // Send to all connected users (for feed updates)
        await Clients.All.SendAsync("NewComment", new { PostId = commentDto.PostId, CommentCount = comment.Id });
        
        // Notify post owner if comment is not from them
        var post = await _postService.GetPostById(commentDto.PostId, userId);
        if (post != null && post.UserId != userId)
        {
            await _notificationService.CreateCommentNotification(post.UserId, post.Id, comment);
        }
        
        // If it's a reply, notify the parent comment owner
        if (commentDto.ParentCommentId.HasValue)
        {
            var parentComment = await _postService.GetComment(commentDto.ParentCommentId.Value);
            if (parentComment != null && parentComment.UserId != userId)
            {
                await _notificationService.CreateReplyNotification(parentComment.UserId, post?.Id ?? 0, comment);
            }
        }
    }

    public async Task LikePost(int postId)
    {
        var userId = GetUserId();
        var result = await _postService.TogglePostLike(userId, postId);
        
        // Broadcast to all users watching this post
        await Clients.Group($"post_{postId}").SendAsync("PostLikeUpdate", new 
        { 
            PostId = postId, 
            LikesCount = result.LikesCount,
            IsLiked = result.IsLiked,
            UserId = userId
        });
        
        // Notify post owner if liked (not if unliked)
        if (result.IsLiked)
        {
            var post = await _postService.GetPostById(postId, userId);
            if (post != null && post.UserId != userId)
            {
                await _notificationService.CreatePostLikeNotification(post.UserId, postId, userId);
            }
        }
    }

    public async Task LikeComment(int commentId)
    {
        var userId = GetUserId();
        var result = await _postService.ToggleCommentLike(userId, commentId);
        
        var comment = await _postService.GetComment(commentId);
        if (comment != null)
        {
            // Broadcast to all users watching this post
            await Clients.Group($"post_{comment.PostId}").SendAsync("CommentLikeUpdate", new 
            { 
                CommentId = commentId, 
                LikesCount = result.LikesCount,
                IsLiked = result.IsLiked,
                UserId = userId
            });
            
            // Notify comment owner if liked
            if (result.IsLiked && comment.UserId != userId)
            {
                await _notificationService.CreateCommentLikeNotification(comment.UserId, commentId, userId);
            }
        }
    }

    public async Task UserTypingComment(int postId, bool isTyping)
    {
        var userId = GetUserId();
        await Clients.OthersInGroup($"post_{postId}")
                     .SendAsync("UserTypingComment", userId, postId, isTyping);
    }

    private int GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}


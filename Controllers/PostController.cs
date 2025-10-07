using ChatApp.Backend.DTOs;
using ChatApp.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IFileUploadService _fileUploadService;
    private readonly INotificationService _notificationService;

    public PostController(
        IPostService postService, 
        IFileUploadService fileUploadService,
        INotificationService notificationService)
    {
        _postService = postService;
        _fileUploadService = fileUploadService;
        _notificationService = notificationService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PostDto>> CreatePost([FromForm] CreatePostDto postDto)
    {
        var userId = GetUserId();
        try
        {
            // Validate file uploads
            if (postDto.MediaFile != null)
            {
                if (postDto.Type == Models.PostType.Image || postDto.Type == Models.PostType.TextWithImage)
                {
                    if (!_fileUploadService.IsImageFile(postDto.MediaFile))
                        return BadRequest(new { message = "Invalid image file. Allowed: jpg, jpeg, png, gif, webp, avif, svg, bmp, tiff, heic" });
                    
                    if (postDto.MediaFile.Length > _fileUploadService.MaxImageSize)
                        return BadRequest(new { message = $"Image file too large. Max size: {_fileUploadService.MaxImageSize / 1024 / 1024}MB" });
                }
                else if (postDto.Type == Models.PostType.Video || postDto.Type == Models.PostType.TextWithVideo)
                {
                    if (!_fileUploadService.IsVideoFile(postDto.MediaFile))
                        return BadRequest(new { message = "Invalid video/audio file. Allowed: mp4, mov, avi, webm, mkv, flv, mp3, wav, ogg, and more" });
                    
                    if (postDto.MediaFile.Length > _fileUploadService.MaxVideoSize)
                        return BadRequest(new { message = $"Video/audio file too large. Max size: {_fileUploadService.MaxVideoSize / 1024 / 1024}MB" });
                }
            }

            // Validate thumbnail for videos
            if (postDto.ThumbnailFile != null)
            {
                if (!_fileUploadService.IsImageFile(postDto.ThumbnailFile))
                    return BadRequest(new { message = "Invalid thumbnail file. Must be an image." });
                
                if (postDto.ThumbnailFile.Length > _fileUploadService.MaxImageSize)
                    return BadRequest(new { message = "Thumbnail file too large." });
            }

            var post = await _postService.CreatePost(userId, postDto);
            return Ok(post);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("feed")]
    public async Task<ActionResult<PostFeedDto>> GetFeed(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var userId = GetUserId();
        var feed = await _postService.GetFeed(userId, skip, take);
        return Ok(feed);
    }

    [HttpGet("{postId}")]
    public async Task<ActionResult<PostDto>> GetPost(int postId)
    {
        var userId = GetUserId();
        var post = await _postService.GetPostById(postId, userId);
        
        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<PostFeedDto>> GetUserPosts(
        int userId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var currentUserId = GetUserId();
        var posts = await _postService.GetUserPosts(userId, currentUserId, skip, take);
        return Ok(posts);
    }

    [HttpPut("{postId}")]
    public async Task<ActionResult<PostDto>> UpdatePost(int postId, [FromBody] UpdatePostDto postDto)
    {
        var userId = GetUserId();
        try
        {
            var post = await _postService.UpdatePost(userId, postId, postDto);
            return Ok(post);
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

    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePost(int postId)
    {
        var userId = GetUserId();
        try
        {
            await _postService.DeletePost(userId, postId);
            return NoContent();
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

    [HttpPost("{postId}/like")]
    public async Task<IActionResult> TogglePostLike(int postId)
    {
        var userId = GetUserId();
        try
        {
            var result = await _postService.TogglePostLike(userId, postId);
            
            // Notify post owner if liked (not if unliked)
            if (result.IsLiked)
            {
                var post = await _postService.GetPostById(postId, userId);
                if (post != null && post.UserId != userId)
                {
                    await _notificationService.CreatePostLikeNotification(post.UserId, postId, userId);
                }
            }
            
            return Ok(new { isLiked = result.IsLiked, likesCount = result.LikesCount });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{postId}/comments")]
    public async Task<ActionResult<List<CommentDto>>> GetComments(
        int postId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var userId = GetUserId();
        var comments = await _postService.GetPostComments(postId, userId, skip, take);
        return Ok(comments);
    }

    [HttpPost("comments")]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto commentDto)
    {
        var userId = GetUserId();
        try
        {
            var comment = await _postService.AddComment(userId, commentDto);
            
            // Notify post owner
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
            
            return Ok(comment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("comments/{commentId}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(int commentId, [FromBody] UpdateCommentDto commentDto)
    {
        var userId = GetUserId();
        try
        {
            var comment = await _postService.UpdateComment(userId, commentId, commentDto);
            return Ok(comment);
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

    [HttpDelete("comments/{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var userId = GetUserId();
        try
        {
            await _postService.DeleteComment(userId, commentId);
            return NoContent();
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

    [HttpPost("comments/{commentId}/like")]
    public async Task<IActionResult> ToggleCommentLike(int commentId)
    {
        var userId = GetUserId();
        try
        {
            var result = await _postService.ToggleCommentLike(userId, commentId);
            
            // Notify comment owner if liked
            if (result.IsLiked)
            {
                var comment = await _postService.GetComment(commentId);
                if (comment != null && comment.UserId != userId)
                {
                    await _notificationService.CreateCommentLikeNotification(comment.UserId, commentId, userId);
                }
            }
            
            return Ok(new { isLiked = result.IsLiked, likesCount = result.LikesCount });
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


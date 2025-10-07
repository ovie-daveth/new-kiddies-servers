using ChatApp.Backend.Models;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Backend.DTOs;

public class CreatePostDto
{
    public string? TextContent { get; set; }
    public PostType Type { get; set; } = PostType.Text;
    public IFormFile? MediaFile { get; set; }       // For image or video upload
    public IFormFile? ThumbnailFile { get; set; }   // For video thumbnail
}

public class UpdatePostDto
{
    public string? TextContent { get; set; }
}

public class PostDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? TextContent { get; set; }
    public PostType Type { get; set; }
    public string? MediaUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsEdited { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
}

public class PostFeedDto
{
    public List<PostDto> Posts { get; set; } = new();
    public int TotalCount { get; set; }
    public bool HasMore { get; set; }
}


namespace ChatApp.Backend.Models;

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? TextContent { get; set; }
    public PostType Type { get; set; } = PostType.Text;
    public string? MediaUrl { get; set; } // URL for image or video
    public string? ThumbnailUrl { get; set; } // Thumbnail for videos
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
}

public enum PostType
{
    Text,
    Image,
    Video,
    TextWithImage,
    TextWithVideo
}


namespace ChatApp.Backend.Models;

public class CommentLike
{
    public int Id { get; set; }
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Comment Comment { get; set; } = null!;
    public User User { get; set; } = null!;
}


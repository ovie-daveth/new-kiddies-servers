namespace ChatApp.Backend.Models;

public class Follow
{
    public int Id { get; set; }
    public int FollowerId { get; set; }  // User who is following
    public int FollowingId { get; set; }  // User being followed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User Follower { get; set; } = null!;
    public User Following { get; set; } = null!;
}


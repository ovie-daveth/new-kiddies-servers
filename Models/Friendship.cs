namespace ChatApp.Backend.Models;

public class Friendship
{
    public int Id { get; set; }
    public int RequesterId { get; set; }  // User who sent the friend request
    public int AddresseeId { get; set; }  // User who received the friend request
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    
    // Navigation properties
    public User Requester { get; set; } = null!;
    public User Addressee { get; set; } = null!;
}

public enum FriendshipStatus
{
    Pending,    // Friend request sent, waiting for response
    Accepted,   // Friend request accepted, now friends
    Rejected,   // Friend request rejected
    Blocked     // User blocked
}


using ChatApp.Backend.Models;

namespace ChatApp.Backend.DTOs;

// User summary for friend lists
public class FriendUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Friend request DTO
public class FriendRequestDto
{
    public int Id { get; set; }
    public FriendUserDto Requester { get; set; } = null!;
    public FriendUserDto Addressee { get; set; } = null!;
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
}

// Send friend request
public class SendFriendRequestDto
{
    public int AddresseeId { get; set; }
}

// Friendship status response
public class FriendshipStatusDto
{
    public bool AreFriends { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsFollowedBy { get; set; }
    public bool HasPendingRequest { get; set; }
    public FriendshipStatus? FriendshipStatus { get; set; }
}

// Friends list response
public class FriendsListDto
{
    public List<FriendUserDto> Friends { get; set; } = new();
    public int TotalCount { get; set; }
}

// Followers/Following list response
public class FollowListDto
{
    public List<FriendUserDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
}

// Friend suggestions
public class FriendSuggestionDto
{
    public FriendUserDto User { get; set; } = null!;
    public int MutualFriendsCount { get; set; }
    public List<FriendUserDto> MutualFriends { get; set; } = new();
}

// User statistics
public class UserStatsDto
{
    public int FriendsCount { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
}

// User profile with relationship info
public class UserProfileDto
{
    public FriendUserDto User { get; set; } = null!;
    public UserStatsDto Stats { get; set; } = null!;
    public FriendshipStatusDto RelationshipStatus { get; set; } = null!;
}


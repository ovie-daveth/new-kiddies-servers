using ChatApp.Backend.Data;
using ChatApp.Backend.DTOs;
using ChatApp.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Backend.Services;

public class FriendService : IFriendService
{
    private readonly ChatDbContext _context;

    public FriendService(ChatDbContext context)
    {
        _context = context;
    }

    // Friend Requests
    public async Task<FriendRequestDto> SendFriendRequest(int requesterId, int addresseeId)
    {
        if (requesterId == addresseeId)
            throw new InvalidOperationException("Cannot send friend request to yourself");

        // Check if users exist
        var requester = await _context.Users.FindAsync(requesterId);
        var addressee = await _context.Users.FindAsync(addresseeId);
        
        if (requester == null || addressee == null)
            throw new InvalidOperationException("User not found");

        // Check if friendship already exists
        var existingFriendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.RequesterId == requesterId && f.AddresseeId == addresseeId) ||
                (f.RequesterId == addresseeId && f.AddresseeId == requesterId));

        if (existingFriendship != null)
        {
            if (existingFriendship.Status == FriendshipStatus.Accepted)
                throw new InvalidOperationException("You are already friends");
            if (existingFriendship.Status == FriendshipStatus.Pending)
                throw new InvalidOperationException("Friend request already pending");
            if (existingFriendship.Status == FriendshipStatus.Blocked)
                throw new InvalidOperationException("Cannot send friend request");
        }

        var friendship = new Friendship
        {
            RequesterId = requesterId,
            AddresseeId = addresseeId,
            Status = FriendshipStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();

        return await GetFriendRequestDto(friendship.Id);
    }

    public async Task<FriendRequestDto> AcceptFriendRequest(int userId, int friendshipId)
    {
        var friendship = await _context.Friendships
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .FirstOrDefaultAsync(f => f.Id == friendshipId);

        if (friendship == null)
            throw new InvalidOperationException("Friend request not found");

        if (friendship.AddresseeId != userId)
            throw new UnauthorizedAccessException("You can only accept friend requests sent to you");

        if (friendship.Status != FriendshipStatus.Pending)
            throw new InvalidOperationException("Friend request is not pending");

        friendship.Status = FriendshipStatus.Accepted;
        friendship.AcceptedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetFriendRequestDto(friendship.Id);
    }

    public async Task RejectFriendRequest(int userId, int friendshipId)
    {
        var friendship = await _context.Friendships.FindAsync(friendshipId);

        if (friendship == null)
            throw new InvalidOperationException("Friend request not found");

        if (friendship.AddresseeId != userId)
            throw new UnauthorizedAccessException("You can only reject friend requests sent to you");

        if (friendship.Status != FriendshipStatus.Pending)
            throw new InvalidOperationException("Friend request is not pending");

        friendship.Status = FriendshipStatus.Rejected;
        await _context.SaveChangesAsync();
    }

    public async Task CancelFriendRequest(int userId, int friendshipId)
    {
        var friendship = await _context.Friendships.FindAsync(friendshipId);

        if (friendship == null)
            throw new InvalidOperationException("Friend request not found");

        if (friendship.RequesterId != userId)
            throw new UnauthorizedAccessException("You can only cancel friend requests you sent");

        if (friendship.Status != FriendshipStatus.Pending)
            throw new InvalidOperationException("Friend request is not pending");

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();
    }

    public async Task<List<FriendRequestDto>> GetPendingFriendRequests(int userId)
    {
        var friendRequests = await _context.Friendships
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .Where(f => f.AddresseeId == userId && f.Status == FriendshipStatus.Pending)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return friendRequests.Select(MapToFriendRequestDto).ToList();
    }

    public async Task<List<FriendRequestDto>> GetSentFriendRequests(int userId)
    {
        var friendRequests = await _context.Friendships
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .Where(f => f.RequesterId == userId && f.Status == FriendshipStatus.Pending)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return friendRequests.Select(MapToFriendRequestDto).ToList();
    }

    // Friends Management
    public async Task RemoveFriend(int userId, int friendId)
    {
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                ((f.RequesterId == userId && f.AddresseeId == friendId) ||
                 (f.RequesterId == friendId && f.AddresseeId == userId)) &&
                f.Status == FriendshipStatus.Accepted);

        if (friendship == null)
            throw new InvalidOperationException("Friendship not found");

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();
    }

    public async Task<FriendsListDto> GetFriends(int userId, int skip = 0, int take = 50)
    {
        var friendships = await _context.Friendships
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .Where(f =>
                (f.RequesterId == userId || f.AddresseeId == userId) &&
                f.Status == FriendshipStatus.Accepted)
            .ToListAsync();

        var friends = friendships.Select(f =>
        {
            var friend = f.RequesterId == userId ? f.Addressee : f.Requester;
            return MapToFriendUserDto(friend);
        })
        .OrderBy(f => f.DisplayName ?? f.Username)
        .Skip(skip)
        .Take(take)
        .ToList();

        return new FriendsListDto
        {
            Friends = friends,
            TotalCount = friendships.Count
        };
    }

    public async Task<FriendsListDto> GetMutualFriends(int userId, int otherUserId)
    {
        // Get friends of both users
        var userFriendIds = await GetFriendIds(userId);
        var otherUserFriendIds = await GetFriendIds(otherUserId);

        // Find mutual friends
        var mutualFriendIds = userFriendIds.Intersect(otherUserFriendIds).ToList();

        var mutualFriends = await _context.Users
            .Where(u => mutualFriendIds.Contains(u.Id))
            .Select(u => MapToFriendUserDto(u))
            .ToListAsync();

        return new FriendsListDto
        {
            Friends = mutualFriends,
            TotalCount = mutualFriends.Count
        };
    }

    public async Task<bool> AreFriends(int userId, int otherUserId)
    {
        return await _context.Friendships
            .AnyAsync(f =>
                ((f.RequesterId == userId && f.AddresseeId == otherUserId) ||
                 (f.RequesterId == otherUserId && f.AddresseeId == userId)) &&
                f.Status == FriendshipStatus.Accepted);
    }

    // Follow/Unfollow
    public async Task<bool> FollowUser(int followerId, int followingId)
    {
        if (followerId == followingId)
            throw new InvalidOperationException("Cannot follow yourself");

        var existingFollow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

        if (existingFollow != null)
            return false; // Already following

        var follow = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task UnfollowUser(int followerId, int followingId)
    {
        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

        if (follow == null)
            throw new InvalidOperationException("Follow relationship not found");

        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync();
    }

    public async Task<FollowListDto> GetFollowers(int userId, int skip = 0, int take = 50)
    {
        var followers = await _context.Follows
            .Include(f => f.Follower)
            .Where(f => f.FollowingId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(f => MapToFriendUserDto(f.Follower))
            .ToListAsync();

        var totalCount = await _context.Follows.CountAsync(f => f.FollowingId == userId);

        return new FollowListDto
        {
            Users = followers,
            TotalCount = totalCount
        };
    }

    public async Task<FollowListDto> GetFollowing(int userId, int skip = 0, int take = 50)
    {
        var following = await _context.Follows
            .Include(f => f.Following)
            .Where(f => f.FollowerId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(f => MapToFriendUserDto(f.Following))
            .ToListAsync();

        var totalCount = await _context.Follows.CountAsync(f => f.FollowerId == userId);

        return new FollowListDto
        {
            Users = following,
            TotalCount = totalCount
        };
    }

    public async Task<bool> IsFollowing(int followerId, int followingId)
    {
        return await _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    // Relationship Status
    public async Task<FriendshipStatusDto> GetRelationshipStatus(int userId, int otherUserId)
    {
        var areFriends = await AreFriends(userId, otherUserId);
        var isFollowing = await IsFollowing(userId, otherUserId);
        var isFollowedBy = await IsFollowing(otherUserId, userId);

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.RequesterId == userId && f.AddresseeId == otherUserId) ||
                (f.RequesterId == otherUserId && f.AddresseeId == userId));

        var hasPendingRequest = friendship?.Status == FriendshipStatus.Pending;

        return new FriendshipStatusDto
        {
            AreFriends = areFriends,
            IsFollowing = isFollowing,
            IsFollowedBy = isFollowedBy,
            HasPendingRequest = hasPendingRequest,
            FriendshipStatus = friendship?.Status
        };
    }

    // User Search & Suggestions
    public async Task<List<FriendUserDto>> SearchUsers(string query, int currentUserId, int skip = 0, int take = 20)
    {
        var normalizedQuery = query.ToLower();

        var users = await _context.Users
            .Where(u =>
                u.Id != currentUserId &&
                (u.Username.ToLower().Contains(normalizedQuery) ||
                 (u.DisplayName != null && u.DisplayName.ToLower().Contains(normalizedQuery))))
            .OrderBy(u => u.Username)
            .Skip(skip)
            .Take(take)
            .Select(u => MapToFriendUserDto(u))
            .ToListAsync();

        return users;
    }

    public async Task<List<FriendSuggestionDto>> GetFriendSuggestions(int userId, int take = 10)
    {
        // Get user's friend IDs
        var userFriendIds = await GetFriendIds(userId);

        // Find users who are friends with user's friends but not friends with user
        var suggestions = await _context.Friendships
            .Where(f =>
                userFriendIds.Contains(f.RequesterId) &&
                f.AddresseeId != userId &&
                !userFriendIds.Contains(f.AddresseeId) &&
                f.Status == FriendshipStatus.Accepted)
            .GroupBy(f => f.AddresseeId)
            .Select(g => new { UserId = g.Key, MutualCount = g.Count() })
            .Union(
                _context.Friendships
                    .Where(f =>
                        userFriendIds.Contains(f.AddresseeId) &&
                        f.RequesterId != userId &&
                        !userFriendIds.Contains(f.RequesterId) &&
                        f.Status == FriendshipStatus.Accepted)
                    .GroupBy(f => f.RequesterId)
                    .Select(g => new { UserId = g.Key, MutualCount = g.Count() })
            )
            .OrderByDescending(x => x.MutualCount)
            .Take(take)
            .ToListAsync();

        var suggestionDtos = new List<FriendSuggestionDto>();

        foreach (var suggestion in suggestions)
        {
            var user = await _context.Users.FindAsync(suggestion.UserId);
            if (user != null)
            {
                var mutualFriends = await GetMutualFriends(userId, suggestion.UserId);

                suggestionDtos.Add(new FriendSuggestionDto
                {
                    User = MapToFriendUserDto(user),
                    MutualFriendsCount = suggestion.MutualCount,
                    MutualFriends = mutualFriends.Friends.Take(3).ToList()
                });
            }
        }

        return suggestionDtos;
    }

    // User Stats
    public async Task<UserStatsDto> GetUserStats(int userId)
    {
        var friendsCount = await _context.Friendships
            .CountAsync(f =>
                (f.RequesterId == userId || f.AddresseeId == userId) &&
                f.Status == FriendshipStatus.Accepted);

        var followersCount = await _context.Follows
            .CountAsync(f => f.FollowingId == userId);

        var followingCount = await _context.Follows
            .CountAsync(f => f.FollowerId == userId);

        var postsCount = await _context.Posts
            .CountAsync(p => p.UserId == userId);

        return new UserStatsDto
        {
            FriendsCount = friendsCount,
            FollowersCount = followersCount,
            FollowingCount = followingCount,
            PostsCount = postsCount
        };
    }

    public async Task<UserProfileDto> GetUserProfile(int userId, int currentUserId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var stats = await GetUserStats(userId);
        var relationshipStatus = await GetRelationshipStatus(currentUserId, userId);

        return new UserProfileDto
        {
            User = MapToFriendUserDto(user),
            Stats = stats,
            RelationshipStatus = relationshipStatus
        };
    }

    // Block User
    public async Task BlockUser(int userId, int blockedUserId)
    {
        if (userId == blockedUserId)
            throw new InvalidOperationException("Cannot block yourself");

        // Remove existing friendship if any
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.RequesterId == userId && f.AddresseeId == blockedUserId) ||
                (f.RequesterId == blockedUserId && f.AddresseeId == userId));

        if (friendship != null)
        {
            friendship.Status = FriendshipStatus.Blocked;
        }
        else
        {
            friendship = new Friendship
            {
                RequesterId = userId,
                AddresseeId = blockedUserId,
                Status = FriendshipStatus.Blocked,
                CreatedAt = DateTime.UtcNow
            };
            _context.Friendships.Add(friendship);
        }

        // Remove follow relationships
        var follows = await _context.Follows
            .Where(f =>
                (f.FollowerId == userId && f.FollowingId == blockedUserId) ||
                (f.FollowerId == blockedUserId && f.FollowingId == userId))
            .ToListAsync();

        _context.Follows.RemoveRange(follows);

        await _context.SaveChangesAsync();
    }

    public async Task UnblockUser(int userId, int blockedUserId)
    {
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                f.RequesterId == userId &&
                f.AddresseeId == blockedUserId &&
                f.Status == FriendshipStatus.Blocked);

        if (friendship == null)
            throw new InvalidOperationException("Block relationship not found");

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();
    }

    public async Task<List<FriendUserDto>> GetBlockedUsers(int userId)
    {
        var blockedUsers = await _context.Friendships
            .Include(f => f.Addressee)
            .Where(f => f.RequesterId == userId && f.Status == FriendshipStatus.Blocked)
            .Select(f => MapToFriendUserDto(f.Addressee))
            .ToListAsync();

        return blockedUsers;
    }

    // Helper methods
    private async Task<List<int>> GetFriendIds(int userId)
    {
        var friendships = await _context.Friendships
            .Where(f =>
                (f.RequesterId == userId || f.AddresseeId == userId) &&
                f.Status == FriendshipStatus.Accepted)
            .ToListAsync();

        return friendships
            .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
            .ToList();
    }

    private async Task<FriendRequestDto> GetFriendRequestDto(int friendshipId)
    {
        var friendship = await _context.Friendships
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .FirstOrDefaultAsync(f => f.Id == friendshipId);

        if (friendship == null)
            throw new InvalidOperationException("Friendship not found");

        return MapToFriendRequestDto(friendship);
    }

    private static FriendRequestDto MapToFriendRequestDto(Friendship friendship)
    {
        return new FriendRequestDto
        {
            Id = friendship.Id,
            Requester = MapToFriendUserDto(friendship.Requester),
            Addressee = MapToFriendUserDto(friendship.Addressee),
            Status = friendship.Status,
            CreatedAt = friendship.CreatedAt,
            AcceptedAt = friendship.AcceptedAt
        };
    }

    private static FriendUserDto MapToFriendUserDto(User user)
    {
        return new FriendUserDto
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Bio = user.Bio,
            IsOnline = user.IsOnline,
            LastSeen = user.LastSeen,
            CreatedAt = user.CreatedAt
        };
    }
}


using ChatApp.Backend.DTOs;
using ChatApp.Backend.Models;

namespace ChatApp.Backend.Services;

public interface IFriendService
{
    // Friend Requests
    Task<FriendRequestDto> SendFriendRequest(int requesterId, int addresseeId);
    Task<FriendRequestDto> AcceptFriendRequest(int userId, int friendshipId);
    Task RejectFriendRequest(int userId, int friendshipId);
    Task CancelFriendRequest(int userId, int friendshipId);
    Task<List<FriendRequestDto>> GetPendingFriendRequests(int userId);
    Task<List<FriendRequestDto>> GetSentFriendRequests(int userId);
    
    // Friends Management
    Task RemoveFriend(int userId, int friendId);
    Task<FriendsListDto> GetFriends(int userId, int skip = 0, int take = 50);
    Task<FriendsListDto> GetMutualFriends(int userId, int otherUserId);
    Task<bool> AreFriends(int userId, int otherUserId);
    
    // Follow/Unfollow
    Task<bool> FollowUser(int followerId, int followingId);
    Task UnfollowUser(int followerId, int followingId);
    Task<FollowListDto> GetFollowers(int userId, int skip = 0, int take = 50);
    Task<FollowListDto> GetFollowing(int userId, int skip = 0, int take = 50);
    Task<bool> IsFollowing(int followerId, int followingId);
    
    // Relationship Status
    Task<FriendshipStatusDto> GetRelationshipStatus(int userId, int otherUserId);
    
    // User Search & Suggestions
    Task<List<FriendUserDto>> SearchUsers(string query, int currentUserId, int skip = 0, int take = 20);
    Task<List<FriendSuggestionDto>> GetFriendSuggestions(int userId, int take = 10);
    
    // User Stats
    Task<UserStatsDto> GetUserStats(int userId);
    Task<UserProfileDto> GetUserProfile(int userId, int currentUserId);
    
    // Block User
    Task BlockUser(int userId, int blockedUserId);
    Task UnblockUser(int userId, int blockedUserId);
    Task<List<FriendUserDto>> GetBlockedUsers(int userId);
}


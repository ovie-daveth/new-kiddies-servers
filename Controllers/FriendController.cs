using ChatApp.Backend.DTOs;
using ChatApp.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FriendController : ControllerBase
{
    private readonly IFriendService _friendService;
    private readonly INotificationService _notificationService;

    public FriendController(IFriendService friendService, INotificationService notificationService)
    {
        _friendService = friendService;
        _notificationService = notificationService;
    }

    #region Friend Requests

    [HttpPost("requests")]
    public async Task<ActionResult<FriendRequestDto>> SendFriendRequest([FromBody] SendFriendRequestDto dto)
    {
        var userId = GetUserId();
        try
        {
            var friendRequest = await _friendService.SendFriendRequest(userId, dto.AddresseeId);
            
            // Send notification
            await _notificationService.CreateFriendRequestNotification(dto.AddresseeId, userId);
            
            return Ok(friendRequest);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("requests/{friendshipId}/accept")]
    public async Task<ActionResult<FriendRequestDto>> AcceptFriendRequest(int friendshipId)
    {
        var userId = GetUserId();
        try
        {
            var friendRequest = await _friendService.AcceptFriendRequest(userId, friendshipId);
            
            // Send notification to requester
            await _notificationService.CreateFriendRequestAcceptedNotification(
                friendRequest.Requester.Id, userId);
            
            return Ok(friendRequest);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("requests/{friendshipId}/reject")]
    public async Task<IActionResult> RejectFriendRequest(int friendshipId)
    {
        var userId = GetUserId();
        try
        {
            await _friendService.RejectFriendRequest(userId, friendshipId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("requests/{friendshipId}")]
    public async Task<IActionResult> CancelFriendRequest(int friendshipId)
    {
        var userId = GetUserId();
        try
        {
            await _friendService.CancelFriendRequest(userId, friendshipId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("requests/pending")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetPendingFriendRequests()
    {
        var userId = GetUserId();
        var requests = await _friendService.GetPendingFriendRequests(userId);
        return Ok(requests);
    }

    [HttpGet("requests/sent")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetSentFriendRequests()
    {
        var userId = GetUserId();
        var requests = await _friendService.GetSentFriendRequests(userId);
        return Ok(requests);
    }

    #endregion

    #region Friends Management

    [HttpDelete("{friendId}")]
    public async Task<IActionResult> RemoveFriend(int friendId)
    {
        var userId = GetUserId();
        try
        {
            await _friendService.RemoveFriend(userId, friendId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<FriendsListDto>> GetFriends(
        [FromQuery] int? userId = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var targetUserId = userId ?? GetUserId();
        var friends = await _friendService.GetFriends(targetUserId, skip, take);
        return Ok(friends);
    }

    [HttpGet("mutual/{userId}")]
    public async Task<ActionResult<FriendsListDto>> GetMutualFriends(int userId)
    {
        var currentUserId = GetUserId();
        var mutualFriends = await _friendService.GetMutualFriends(currentUserId, userId);
        return Ok(mutualFriends);
    }

    [HttpGet("check/{userId}")]
    public async Task<ActionResult<bool>> AreFriends(int userId)
    {
        var currentUserId = GetUserId();
        var areFriends = await _friendService.AreFriends(currentUserId, userId);
        return Ok(new { areFriends });
    }

    #endregion

    #region Follow/Unfollow

    [HttpPost("follow/{userId}")]
    public async Task<IActionResult> FollowUser(int userId)
    {
        var currentUserId = GetUserId();
        try
        {
            var followed = await _friendService.FollowUser(currentUserId, userId);
            
            if (followed)
            {
                // Send notification
                await _notificationService.CreateNewFollowerNotification(userId, currentUserId);
                return Ok(new { message = "Successfully followed user", isFollowing = true });
            }
            
            return Ok(new { message = "Already following this user", isFollowing = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("follow/{userId}")]
    public async Task<IActionResult> UnfollowUser(int userId)
    {
        var currentUserId = GetUserId();
        try
        {
            await _friendService.UnfollowUser(currentUserId, userId);
            return Ok(new { message = "Successfully unfollowed user", isFollowing = false });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId}/followers")]
    public async Task<ActionResult<FollowListDto>> GetFollowers(
        int userId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var followers = await _friendService.GetFollowers(userId, skip, take);
        return Ok(followers);
    }

    [HttpGet("{userId}/following")]
    public async Task<ActionResult<FollowListDto>> GetFollowing(
        int userId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var following = await _friendService.GetFollowing(userId, skip, take);
        return Ok(following);
    }

    [HttpGet("following/check/{userId}")]
    public async Task<ActionResult<bool>> IsFollowing(int userId)
    {
        var currentUserId = GetUserId();
        var isFollowing = await _friendService.IsFollowing(currentUserId, userId);
        return Ok(new { isFollowing });
    }

    #endregion

    #region Relationship Status

    [HttpGet("status/{userId}")]
    public async Task<ActionResult<FriendshipStatusDto>> GetRelationshipStatus(int userId)
    {
        var currentUserId = GetUserId();
        var status = await _friendService.GetRelationshipStatus(currentUserId, userId);
        return Ok(status);
    }

    #endregion

    #region User Search & Suggestions

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ActionResult<List<FriendUserDto>>> SearchUsers(
        [FromQuery] string query,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return BadRequest(new { message = "Search query must be at least 2 characters" });
        }

        var currentUserId = GetUserIdOrDefault();
        var users = await _friendService.SearchUsers(query, currentUserId, skip, take);
        return Ok(users);
    }

    [HttpGet("suggestions")]
    public async Task<ActionResult<List<FriendSuggestionDto>>> GetFriendSuggestions([FromQuery] int take = 10)
    {
        var userId = GetUserId();
        var suggestions = await _friendService.GetFriendSuggestions(userId, take);
        return Ok(suggestions);
    }

    #endregion

    #region User Profile & Stats

    [AllowAnonymous]
    [HttpGet("profile/{userId}")]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(int userId)
    {
        var currentUserId = GetUserIdOrDefault();
        try
        {
            var profile = await _friendService.GetUserProfile(userId, currentUserId);
            return Ok(profile);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("stats/{userId}")]
    public async Task<ActionResult<UserStatsDto>> GetUserStats(int userId)
    {
        var stats = await _friendService.GetUserStats(userId);
        return Ok(stats);
    }

    #endregion

    #region Block User

    [HttpPost("block/{userId}")]
    public async Task<IActionResult> BlockUser(int userId)
    {
        var currentUserId = GetUserId();
        try
        {
            await _friendService.BlockUser(currentUserId, userId);
            return Ok(new { message = "User blocked successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("block/{userId}")]
    public async Task<IActionResult> UnblockUser(int userId)
    {
        var currentUserId = GetUserId();
        try
        {
            await _friendService.UnblockUser(currentUserId, userId);
            return Ok(new { message = "User unblocked successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("blocked")]
    public async Task<ActionResult<List<FriendUserDto>>> GetBlockedUsers()
    {
        var userId = GetUserId();
        var blockedUsers = await _friendService.GetBlockedUsers(userId);
        return Ok(blockedUsers);
    }

    #endregion

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private int GetUserIdOrDefault()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? int.Parse(userIdClaim) : 0;
    }
}


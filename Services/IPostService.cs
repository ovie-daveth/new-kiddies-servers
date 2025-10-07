using ChatApp.Backend.DTOs;

namespace ChatApp.Backend.Services;

public interface IPostService
{
    Task<PostDto> CreatePost(int userId, CreatePostDto postDto);
    Task<PostDto?> GetPostById(int postId, int currentUserId);
    Task<PostFeedDto> GetFeed(int userId, int skip = 0, int take = 20);
    Task<PostFeedDto> GetUserPosts(int userId, int currentUserId, int skip = 0, int take = 20);
    Task<PostDto> UpdatePost(int userId, int postId, UpdatePostDto postDto);
    Task DeletePost(int userId, int postId);
    
    Task<CommentDto> AddComment(int userId, CreateCommentDto commentDto);
    Task<CommentDto?> GetComment(int commentId);
    Task<List<CommentDto>> GetPostComments(int postId, int currentUserId, int skip = 0, int take = 50);
    Task<CommentDto> UpdateComment(int userId, int commentId, UpdateCommentDto commentDto);
    Task DeleteComment(int userId, int commentId);
    
    Task<(bool IsLiked, int LikesCount)> TogglePostLike(int userId, int postId);
    Task<(bool IsLiked, int LikesCount)> ToggleCommentLike(int userId, int commentId);
}


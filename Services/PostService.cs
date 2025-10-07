using ChatApp.Backend.Data;
using ChatApp.Backend.DTOs;
using ChatApp.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Backend.Services;

public class PostService : IPostService
{
    private readonly ChatDbContext _context;
    private readonly IFileUploadService _fileUploadService;

    public PostService(ChatDbContext context, IFileUploadService fileUploadService)
    {
        _context = context;
        _fileUploadService = fileUploadService;
    }

    public async Task<PostDto> CreatePost(int userId, CreatePostDto postDto)
    {
        string? mediaUrl = null;
        string? thumbnailUrl = null;

        // Upload media file if provided
        if (postDto.MediaFile != null)
        {
            var folder = postDto.Type == PostType.Video || postDto.Type == PostType.TextWithVideo ? "videos" : "images";
            mediaUrl = await _fileUploadService.SaveFileAsync(postDto.MediaFile, folder);
        }

        // Upload thumbnail if provided (for videos)
        if (postDto.ThumbnailFile != null)
        {
            thumbnailUrl = await _fileUploadService.SaveFileAsync(postDto.ThumbnailFile, "thumbnails");
        }

        var post = new Post
        {
            UserId = userId,
            TextContent = postDto.TextContent,
            Type = postDto.Type,
            MediaUrl = mediaUrl,
            ThumbnailUrl = thumbnailUrl,
            CreatedAt = DateTime.UtcNow,
            LikesCount = 0,
            CommentsCount = 0
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return await MapToPostDto(post, userId);
    }

    public async Task<PostDto?> GetPostById(int postId, int currentUserId)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

        if (post == null) return null;

        return await MapToPostDto(post, currentUserId);
    }

    public async Task<PostFeedDto> GetFeed(int userId, int skip = 0, int take = 20)
    {
        var totalCount = await _context.Posts.CountAsync(p => !p.IsDeleted);

        var posts = await _context.Posts
            .Include(p => p.User)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var postDtos = new List<PostDto>();
        foreach (var post in posts)
        {
            postDtos.Add(await MapToPostDto(post, userId));
        }

        return new PostFeedDto
        {
            Posts = postDtos,
            TotalCount = totalCount,
            HasMore = skip + take < totalCount
        };
    }

    public async Task<PostFeedDto> GetUserPosts(int userId, int currentUserId, int skip = 0, int take = 20)
    {
        var totalCount = await _context.Posts.CountAsync(p => p.UserId == userId && !p.IsDeleted);

        var posts = await _context.Posts
            .Include(p => p.User)
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var postDtos = new List<PostDto>();
        foreach (var post in posts)
        {
            postDtos.Add(await MapToPostDto(post, currentUserId));
        }

        return new PostFeedDto
        {
            Posts = postDtos,
            TotalCount = totalCount,
            HasMore = skip + take < totalCount
        };
    }

    public async Task<PostDto> UpdatePost(int userId, int postId, UpdatePostDto postDto)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

        if (post == null)
            throw new Exception("Post not found");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own posts");

        post.TextContent = postDto.TextContent;
        post.IsEdited = true;
        post.EditedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await MapToPostDto(post, userId);
    }

    public async Task DeletePost(int userId, int postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            throw new Exception("Post not found");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own posts");

        // Delete associated files
        if (!string.IsNullOrEmpty(post.MediaUrl))
        {
            await _fileUploadService.DeleteFileAsync(post.MediaUrl);
        }

        if (!string.IsNullOrEmpty(post.ThumbnailUrl))
        {
            await _fileUploadService.DeleteFileAsync(post.ThumbnailUrl);
        }

        post.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<CommentDto> AddComment(int userId, CreateCommentDto commentDto)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == commentDto.PostId && !p.IsDeleted);
        if (post == null)
            throw new Exception("Post not found");

        // Validate parent comment if provided
        if (commentDto.ParentCommentId.HasValue)
        {
            var parentComment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentDto.ParentCommentId.Value && !c.IsDeleted);
            
            if (parentComment == null)
                throw new Exception("Parent comment not found");
            
            if (parentComment.PostId != commentDto.PostId)
                throw new Exception("Parent comment does not belong to this post");
        }

        var comment = new Comment
        {
            PostId = commentDto.PostId,
            UserId = userId,
            ParentCommentId = commentDto.ParentCommentId,
            Content = commentDto.Content,
            CreatedAt = DateTime.UtcNow,
            LikesCount = 0
        };

        _context.Comments.Add(comment);

        // Update post comments count
        post.CommentsCount++;

        await _context.SaveChangesAsync();

        return await MapToCommentDto(comment, userId);
    }

    public async Task<CommentDto?> GetComment(int commentId)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null) return null;

        return await MapToCommentDto(comment, comment.UserId);
    }

    public async Task<List<CommentDto>> GetPostComments(int postId, int currentUserId, int skip = 0, int take = 50)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Replies.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.User)
            .Where(c => c.PostId == postId && !c.IsDeleted && c.ParentCommentId == null)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var commentDtos = new List<CommentDto>();
        foreach (var comment in comments)
        {
            var commentDto = await MapToCommentDto(comment, currentUserId);
            
            // Map replies
            foreach (var reply in comment.Replies.Where(r => !r.IsDeleted))
            {
                commentDto.Replies.Add(await MapToCommentDto(reply, currentUserId));
            }
            
            commentDtos.Add(commentDto);
        }

        return commentDtos;
    }

    public async Task<CommentDto> UpdateComment(int userId, int commentId, UpdateCommentDto commentDto)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null)
            throw new Exception("Comment not found");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own comments");

        comment.Content = commentDto.Content;
        comment.IsEdited = true;
        comment.EditedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await MapToCommentDto(comment, userId);
    }

    public async Task DeleteComment(int userId, int commentId)
    {
        var comment = await _context.Comments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
            throw new Exception("Comment not found");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own comments");

        comment.IsDeleted = true;
        
        // Update post comments count
        if (comment.Post != null)
        {
            comment.Post.CommentsCount--;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<(bool IsLiked, int LikesCount)> TogglePostLike(int userId, int postId)
    {
        var existingLike = await _context.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
            throw new Exception("Post not found");

        if (existingLike != null)
        {
            // Unlike
            _context.PostLikes.Remove(existingLike);
            post.LikesCount--;
            await _context.SaveChangesAsync();
            return (false, post.LikesCount);
        }
        else
        {
            // Like
            var like = new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.PostLikes.Add(like);
            post.LikesCount++;
            await _context.SaveChangesAsync();
            return (true, post.LikesCount);
        }
    }

    public async Task<(bool IsLiked, int LikesCount)> ToggleCommentLike(int userId, int commentId)
    {
        var existingLike = await _context.CommentLikes
            .FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
            throw new Exception("Comment not found");

        if (existingLike != null)
        {
            // Unlike
            _context.CommentLikes.Remove(existingLike);
            comment.LikesCount--;
            await _context.SaveChangesAsync();
            return (false, comment.LikesCount);
        }
        else
        {
            // Like
            var like = new CommentLike
            {
                CommentId = commentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.CommentLikes.Add(like);
            comment.LikesCount++;
            await _context.SaveChangesAsync();
            return (true, comment.LikesCount);
        }
    }

    private async Task<PostDto> MapToPostDto(Post post, int currentUserId)
    {
        var user = post.User ?? await _context.Users.FindAsync(post.UserId);
        var isLiked = await _context.PostLikes
            .AnyAsync(pl => pl.PostId == post.Id && pl.UserId == currentUserId);

        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Username = user?.Username ?? "",
            DisplayName = user?.DisplayName,
            ProfilePictureUrl = user?.ProfilePictureUrl,
            TextContent = post.TextContent,
            Type = post.Type,
            MediaUrl = post.MediaUrl,
            ThumbnailUrl = post.ThumbnailUrl,
            CreatedAt = post.CreatedAt,
            EditedAt = post.EditedAt,
            IsEdited = post.IsEdited,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            IsLikedByCurrentUser = isLiked
        };
    }

    private async Task<CommentDto> MapToCommentDto(Comment comment, int currentUserId)
    {
        var user = comment.User ?? await _context.Users.FindAsync(comment.UserId);
        var isLiked = await _context.CommentLikes
            .AnyAsync(cl => cl.CommentId == comment.Id && cl.UserId == currentUserId);

        return new CommentDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Username = user?.Username ?? "",
            DisplayName = user?.DisplayName,
            ProfilePictureUrl = user?.ProfilePictureUrl,
            ParentCommentId = comment.ParentCommentId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            EditedAt = comment.EditedAt,
            IsEdited = comment.IsEdited,
            LikesCount = comment.LikesCount,
            IsLikedByCurrentUser = isLiked
        };
    }
}


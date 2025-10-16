using ChatApp.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Backend.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Follow> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).HasMaxLength(100);
        });

        // Conversation configuration
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        // ConversationParticipant configuration
        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ConversationId, e.UserId }).IsUnique();
            
            entity.HasOne(e => e.Conversation)
                  .WithMany(c => c.Participants)
                  .HasForeignKey(e => e.ConversationId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Conversations)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Message configuration
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(4000);
            
            entity.HasOne(e => e.Conversation)
                  .WithMany(c => c.Messages)
                  .HasForeignKey(e => e.ConversationId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Sender)
                  .WithMany(u => u.SentMessages)
                  .HasForeignKey(e => e.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Actor)
                  .WithMany()
                  .HasForeignKey(e => e.ActorUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Post configuration
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TextContent).HasMaxLength(5000);
            entity.Property(e => e.MediaUrl).HasMaxLength(500);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.HasIndex(e => e.CreatedAt);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Comment configuration
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.HasIndex(e => e.PostId);
            entity.HasIndex(e => e.CreatedAt);
            
            entity.HasOne(e => e.Post)
                  .WithMany(p => p.Comments)
                  .HasForeignKey(e => e.PostId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ParentComment)
                  .WithMany(c => c.Replies)
                  .HasForeignKey(e => e.ParentCommentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // PostLike configuration
        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PostId, e.UserId }).IsUnique();
            
            entity.HasOne(e => e.Post)
                  .WithMany(p => p.Likes)
                  .HasForeignKey(e => e.PostId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CommentLike configuration
        modelBuilder.Entity<CommentLike>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CommentId, e.UserId }).IsUnique();
            
            entity.HasOne(e => e.Comment)
                  .WithMany(c => c.Likes)
                  .HasForeignKey(e => e.CommentId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Friendship configuration
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RequesterId, e.AddresseeId }).IsUnique();
            entity.HasIndex(e => e.Status);
            
            entity.HasOne(e => e.Requester)
                  .WithMany(u => u.SentFriendRequests)
                  .HasForeignKey(e => e.RequesterId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Addressee)
                  .WithMany(u => u.ReceivedFriendRequests)
                  .HasForeignKey(e => e.AddresseeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Follow configuration
        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }).IsUnique();
            
            entity.HasOne(e => e.Follower)
                  .WithMany(u => u.Following)
                  .HasForeignKey(e => e.FollowerId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Following)
                  .WithMany(u => u.Followers)
                  .HasForeignKey(e => e.FollowingId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}


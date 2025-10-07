# Kids Social Media Platform - Backend

A **mini social media platform for kids** with real-time features built with **ASP.NET Core 8.0** and **SignalR**. Kids can create posts (text, images, videos), comment on posts with live updates, like content, and chat with each other - all with real-time notifications!

---

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| **[📖 DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md)** | **START HERE** - Complete documentation guide |
| [🚀 QUICKSTART.md](QUICKSTART.md) | Get started in 3 minutes |
| [🧪 SETUP_AND_TEST.md](SETUP_AND_TEST.md) | Detailed setup and testing guide |
| [📋 API_DOCUMENTATION.md](API_DOCUMENTATION.md) | **Complete API reference** with all endpoints |
| [⚡ API_QUICK_REFERENCE.md](API_QUICK_REFERENCE.md) | Quick lookup table for developers |
| [💻 API_EXAMPLES.md](API_EXAMPLES.md) | cURL & JavaScript code examples |

---

## 🚀 Features

### Social Media Features
- ✅ **Create Posts** - Share text, images, or videos
- ✅ **Live Comments** - Real-time commenting on posts like a group chat
- ✅ **Nested Replies** - Reply to specific comments
- ✅ **Like Posts & Comments** - Interactive engagement
- ✅ **User Feed** - Scroll through all posts
- ✅ **User Profiles** - View posts by specific users
- ✅ **Real-time Updates** - See new comments and likes instantly

### Direct Messaging
- ✅ **Real-time Chat** - Instant messaging using SignalR
- ✅ **Direct Messages** - One-on-one conversations
- ✅ **Group Chats** - Multi-user group conversations
- ✅ **Typing Indicators** - See when users are typing
- ✅ **Message Read Receipts** - Track read/unread messages

### User Features
- ✅ **User Authentication** - JWT-based secure authentication
- ✅ **User Profiles** - Display names and profile pictures
- ✅ **Online Status** - Track user online/offline status
- ✅ **User Search** - Find and connect with other kids

### Notifications
- ✅ **Live Notifications** - Real-time notification system
- ✅ **Post Comments** - Get notified of new comments
- ✅ **Comment Replies** - Get notified of replies to your comments
- ✅ **Likes** - Get notified when someone likes your content
- ✅ **Messages** - Get notified of new messages
- ✅ **Unread Counts** - Badge notifications for unread items

## 🛠️ Technology Stack

- **ASP.NET Core 8.0** - Web API framework
- **SignalR** - Real-time bidirectional communication
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **JWT Authentication** - Secure token-based authentication
- **BCrypt** - Password hashing
- **Swagger/OpenAPI** - API documentation

## 📋 Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 / VS Code / Rider (optional)

## 🔧 Installation & Setup

### 1. Clone or Navigate to the Repository

```bash
cd new-kiddies-servers
```

### 2. Update Database Connection String

Edit `appsettings.json` and update the connection string to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ChatAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For SQL Server Express:
```
Server=localhost\\SQLEXPRESS;Database=ChatAppDb;Trusted_Connection=True;TrustServerCertificate=True;
```

For SQL Server with username/password:
```
Server=localhost;Database=ChatAppDb;User Id=your_username;Password=your_password;TrustServerCertificate=True;
```

### 3. Update JWT Secret Key

In `appsettings.json`, change the JWT secret key to a secure random string (minimum 32 characters):

```json
{
  "JwtSettings": {
    "SecretKey": "YOUR_SECURE_RANDOM_STRING_HERE_MINIMUM_32_CHARACTERS"
  }
}
```

### 4. Install Dependencies

```bash
dotnet restore
```

### 5. Create Database Migration

```bash
dotnet ef migrations add InitialCreate
```

### 6. Update Database

```bash
dotnet ef database update
```

### 7. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:7001`
- Swagger UI: `https://localhost:7001/swagger`

## 📡 API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token
- `GET /api/auth/me` - Get current user info (requires auth)
- `GET /api/auth/users/search?query={query}` - Search users (requires auth)
- `GET /api/auth/users/{userId}` - Get user by ID (requires auth)

### Posts

- `POST /api/post` - Create a new post
- `GET /api/post/feed` - Get feed of all posts (paginated)
- `GET /api/post/{postId}` - Get specific post
- `GET /api/post/user/{userId}` - Get posts by specific user
- `PUT /api/post/{postId}` - Update a post (owner only)
- `DELETE /api/post/{postId}` - Delete a post (owner only)
- `POST /api/post/{postId}/like` - Like/unlike a post

### Comments

- `GET /api/post/{postId}/comments` - Get comments for a post
- `POST /api/post/comments` - Add a comment
- `PUT /api/post/comments/{commentId}` - Update a comment (owner only)
- `DELETE /api/post/comments/{commentId}` - Delete a comment (owner only)
- `POST /api/post/comments/{commentId}/like` - Like/unlike a comment

### Direct Messages

- `POST /api/chat/conversations` - Create a new conversation
- `GET /api/chat/conversations` - Get all user conversations
- `GET /api/chat/conversations/{id}` - Get specific conversation
- `GET /api/chat/conversations/{id}/messages` - Get conversation messages
- `POST /api/chat/messages` - Send a message (also use SignalR for real-time)

### Notifications

- `GET /api/notification` - Get user notifications
- `GET /api/notification/unread-count` - Get unread count
- `PUT /api/notification/{id}/read` - Mark notification as read
- `PUT /api/notification/mark-all-read` - Mark all as read

## 🔌 SignalR Hubs

### Post Hub (`/hubs/post`) - For Social Media Features

**Events to Send:**
- `JoinPost(postId)` - Join a post room to receive live updates
- `LeavePost(postId)` - Leave a post room
- `SendComment(createCommentDto)` - Post a comment in real-time
- `LikePost(postId)` - Like/unlike a post
- `LikeComment(commentId)` - Like/unlike a comment
- `UserTypingComment(postId, isTyping)` - Send typing status

**Events to Receive:**
- `ReceiveComment(comment)` - New comment posted
- `NewComment({ postId, commentCount })` - Comment count updated
- `PostLikeUpdate({ postId, likesCount, isLiked, userId })` - Post like update
- `CommentLikeUpdate({ commentId, likesCount, isLiked, userId })` - Comment like update
- `UserTypingComment(userId, postId, isTyping)` - User typing in comments

### Chat Hub (`/hubs/chat`) - For Direct Messaging

**Events to Send:**
- `SendMessage(messageDto)` - Send a message
- `JoinConversation(conversationId)` - Join a conversation room
- `LeaveConversation(conversationId)` - Leave a conversation room
- `TypingIndicator(conversationId, isTyping)` - Send typing status
- `MarkMessageAsRead(messageId)` - Mark a message as read

**Events to Receive:**
- `ReceiveMessage(message)` - New message received
- `MessageSent(message)` - Confirmation of sent message
- `UserOnline(userId)` - User came online
- `UserOffline(userId)` - User went offline
- `UserJoinedConversation(userId, conversationId)` - User joined
- `UserLeftConversation(userId, conversationId)` - User left
- `UserTyping(userId, conversationId, isTyping)` - User typing status
- `MessageRead(messageId, userId)` - Message read receipt

### Notification Hub (`/hubs/notification`)

**Events to Send:**
- `MarkNotificationAsRead(notificationId)` - Mark as read
- `MarkAllAsRead()` - Mark all as read

**Events to Receive:**
- `ReceiveNotification(notification)` - New notification
- `UnreadNotificationsCount(count)` - Unread count update

## 🔐 Authentication

All API endpoints (except register and login) require JWT authentication.

### Getting a Token:

1. Register or login:
```json
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "password123"
}
```

2. Use the returned token in subsequent requests:
```
Authorization: Bearer {your_jwt_token}
```

### SignalR Authentication:

When connecting to SignalR hubs, pass the token as a query parameter:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/post?access_token=YOUR_JWT_TOKEN")
    .build();
```

## 📱 Client Integration Examples

### Social Media - Real-time Post Comments (JavaScript/TypeScript)

```javascript
// Install: npm install @microsoft/signalr

import * as signalR from "@microsoft/signalr";

// Connect to Post Hub
const postConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/post?access_token=" + token)
    .withAutomaticReconnect()
    .build();

// Join a specific post to get live updates
await postConnection.invoke("JoinPost", postId);

// Listen for new comments
postConnection.on("ReceiveComment", (comment) => {
    console.log("New comment:", comment);
    // Add comment to UI in real-time
    addCommentToUI(comment);
});

// Listen for like updates
postConnection.on("PostLikeUpdate", (data) => {
    console.log("Post liked:", data);
    updateLikeCount(data.postId, data.likesCount);
});

// Send a comment
await postConnection.invoke("SendComment", {
    postId: 1,
    content: "Great post!",
    parentCommentId: null // or comment ID for replies
});

// Like a post
await postConnection.invoke("LikePost", postId);

// Show typing indicator
await postConnection.invoke("UserTypingComment", postId, true);

// Start connection
await postConnection.start();
```

### Creating Posts with Media

```javascript
// Create a text post
const textPost = await fetch('https://localhost:7001/api/post', {
    method: 'POST',
    headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        textContent: "Check out this awesome post!",
        type: 0 // Text
    })
});

// Create a post with image
const imagePost = await fetch('https://localhost:7001/api/post', {
    method: 'POST',
    headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        textContent: "Look at this cool picture!",
        type: 1, // Image
        mediaUrl: "https://example.com/image.jpg"
    })
});

// Create a post with video
const videoPost = await fetch('https://localhost:7001/api/post', {
    method: 'POST',
    headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        textContent: "Amazing video!",
        type: 2, // Video
        mediaUrl: "https://example.com/video.mp4",
        thumbnailUrl: "https://example.com/thumb.jpg"
    })
});
```

### Direct Messaging Example

```javascript
// Connect to Chat Hub
const chatConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/chat?access_token=" + token)
    .withAutomaticReconnect()
    .build();

// Listen for messages
chatConnection.on("ReceiveMessage", (message) => {
    console.log("New message:", message);
});

// Start connection
await chatConnection.start();

// Send a message
await chatConnection.invoke("SendMessage", {
    conversationId: 1,
    content: "Hello!",
    type: 0 // Text
});
```

## 📊 Database Schema

### Tables:
- **Users** - User accounts and profiles
- **Posts** - Social media posts (text/image/video)
- **Comments** - Comments on posts (with nested replies)
- **PostLikes** - Likes on posts
- **CommentLikes** - Likes on comments
- **Conversations** - Chat conversations (direct or group)
- **ConversationParticipants** - Many-to-many relationship
- **Messages** - Chat messages
- **Notifications** - User notifications

### Post Types:
- `0` - Text only
- `1` - Image
- `2` - Video
- `3` - Text with Image
- `4` - Text with Video

## 🔨 Development

### Install EF Core Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
```

### Create a New Migration

```bash
dotnet ef migrations add MigrationName
```

### Apply Migrations

```bash
dotnet ef database update
```

### Remove Last Migration

```bash
dotnet ef migrations remove
```

## 🚀 Production Deployment

1. Update `appsettings.json` with production database connection string
2. Change JWT SecretKey to a secure production key
3. Update CORS policy in `Program.cs` with your frontend URLs
4. Build the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
5. Deploy the `publish` folder to your server

## 🧪 Testing with Swagger

1. Navigate to `https://localhost:7001/swagger`
2. Register a user via `/api/auth/register`
3. Login via `/api/auth/login` and copy the token
4. Click "Authorize" button and paste the token
5. Test the endpoints

## 📝 Environment Variables

You can also configure settings using environment variables:

- `ConnectionStrings__DefaultConnection` - Database connection string
- `JwtSettings__SecretKey` - JWT secret key
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience

## 🎨 Typical User Flow

1. **Sign Up/Login** - Kid creates account or logs in
2. **Browse Feed** - See posts from all users
3. **Create Post** - Share text, image, or video
4. **Live Comments** - Comment on posts with real-time updates
5. **Like & Reply** - Like posts/comments and reply to comments
6. **Get Notifications** - Receive instant notifications for interactions
7. **Direct Message** - Chat privately with other kids
8. **View Profile** - See their own posts and stats

## 🔒 Safety Features to Consider

While this backend provides the core functionality, for a kids' platform you should consider adding:

- Content moderation and filtering
- Parental controls and monitoring
- Report/block user functionality
- Age verification
- Privacy settings
- Inappropriate content detection
- Rate limiting to prevent spam

## 🤝 Contributing

Feel free to submit issues and enhancement requests!

## 📄 License

This project is open source and available under the MIT License.

## 🆘 Support

For issues and questions, please create an issue in the repository.

---

**Happy Coding! 🎉 Build something amazing for kids!**

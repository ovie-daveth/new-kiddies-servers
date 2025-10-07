# Quick Start Guide - Kids Social Media Platform

## 🚀 Get Started in 3 Minutes!

### Step 1: Setup Database

```bash
# Install EF Core tools (if you haven't already)
dotnet tool install --global dotnet-ef

# Create the database
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 2: Run the Application

```bash
dotnet run
```

Visit: **https://localhost:7001/swagger**

### Step 3: Try It Out!

1. **Register a User**
   - Go to `/api/auth/register` in Swagger
   - Create a test user:
   ```json
   {
     "username": "johnny",
     "email": "johnny@example.com",
     "password": "Test123!",
     "displayName": "Johnny"
   }
   ```

2. **Login and Get Token**
   - Go to `/api/auth/login`
   - Login with your credentials
   - **Copy the token** from the response

3. **Authorize**
   - Click the **Authorize** button (🔒) at the top
   - Paste your token
   - Click "Authorize"

4. **Create Your First Post**
   - Go to `POST /api/post`
   - Try this:
   ```json
   {
     "textContent": "Hello world! My first post!",
     "type": 0
   }
   ```

5. **Test Real-time Features**
   - Create another user
   - Post a comment on the first post
   - Watch the magic happen in real-time!

## 📱 What Can You Build With This?

### Social Media Features
- Instagram-like feed with posts
- TikTok-style video posts
- YouTube-style comments section
- Facebook-like interactions (likes, comments, replies)

### Messaging Features
- WhatsApp-style direct messaging
- Discord-like group chats
- Snapchat-style real-time updates

## 🎮 Sample Use Cases

### 1. School Social Network
Kids share homework, school events, and connect with classmates.

### 2. Gaming Community
Kids share game clips, screenshots, and chat about their favorite games.

### 3. Art & Creative Platform
Kids share their drawings, crafts, and creative projects.

### 4. Learning Platform
Educational content with interactive discussions in comments.

## 🔌 SignalR Hub Quick Reference

### Post Hub - `/hubs/post`
For real-time post interactions

```javascript
// Connect
const hub = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/post?access_token=" + token)
    .build();

// Join a post
await hub.invoke("JoinPost", postId);

// Listen for comments
hub.on("ReceiveComment", (comment) => {
    console.log("New comment!", comment);
});

// Post a comment
await hub.invoke("SendComment", {
    postId: 1,
    content: "Great post!",
    parentCommentId: null
});
```

### Chat Hub - `/hubs/chat`
For real-time direct messaging

```javascript
// Connect
const chat = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/chat?access_token=" + token)
    .build();

// Send message
await chat.invoke("SendMessage", {
    conversationId: 1,
    content: "Hi there!",
    type: 0
});
```

## 📊 Database Overview

The platform creates these tables:
- **Users** - All registered users
- **Posts** - Social media posts
- **Comments** - Comments and replies
- **PostLikes** & **CommentLikes** - Engagement data
- **Conversations** & **Messages** - Direct messaging
- **Notifications** - Real-time notifications

## 🎯 Next Steps

1. **Build a Frontend** - Use React, Vue, Angular, or any framework
2. **Add File Upload** - Implement media upload for images/videos
3. **Add Moderation** - Content filtering and user reporting
4. **Enhance Security** - Add rate limiting, validation, etc.
5. **Deploy** - Host on Azure, AWS, or your preferred platform

## 💡 Tips

- Use Swagger to test all endpoints before building frontend
- Check the main README.md for complete documentation
- All timestamps are in UTC
- Images/videos URLs can point to cloud storage (Azure Blob, AWS S3, etc.)
- Consider implementing content moderation for safety

## 🆘 Common Issues

**"Connection string error"**
- Update the connection string in `appsettings.json` to match your SQL Server

**"Migration error"**
- Delete the `Migrations` folder
- Run `dotnet ef migrations add InitialCreate` again

**"JWT error"**
- Make sure your JWT secret is at least 32 characters

**"CORS error"**
- Add your frontend URL to the CORS policy in `Program.cs`

---

**Have fun building! 🎉**


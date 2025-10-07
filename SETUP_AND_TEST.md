# Setup and Testing Guide

## 📋 Prerequisites Check

Before starting, make sure you have:
- ✅ .NET 8.0 SDK installed - Check with: `dotnet --version`
- ✅ SQL Server (LocalDB, Express, or Full version)

## 🚀 Step-by-Step Setup

### Step 1: Install Entity Framework Tools

```bash
dotnet tool install --global dotnet-ef
```

If already installed, update it:
```bash
dotnet tool update --global dotnet-ef
```

### Step 2: Configure Database Connection

Open `appsettings.json` and verify/update your connection string:

**For SQL Server LocalDB (comes with Visual Studio):**
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=KidsSocialMediaDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

**For SQL Server Express:**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=KidsSocialMediaDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

**For SQL Server with credentials:**
```json
"DefaultConnection": "Server=localhost;Database=KidsSocialMediaDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

### Step 3: Restore NuGet Packages

```bash
dotnet restore
```

### Step 4: Create Database Migration

```bash
dotnet ef migrations add InitialCreate
```

You should see output like:
```
Build started...
Build succeeded.
Done. To undo this action, use 'dotnet ef migrations remove'
```

### Step 5: Create the Database

```bash
dotnet ef database update
```

You should see:
```
Build started...
Build succeeded.
Applying migration 'InitialCreate'.
Done.
```

### Step 6: Run the Application

```bash
dotnet run
```

You should see:
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

🎉 **Your API is now running!**

## 🧪 Testing with Swagger

### 1. Open Swagger UI

Open your browser and go to:
```
https://localhost:7001/swagger
```

You'll see all your API endpoints!

### 2. Register a User

1. Find `POST /api/auth/register`
2. Click "Try it out"
3. Enter this JSON:
```json
{
  "username": "sarah",
  "email": "sarah@example.com",
  "password": "Test123!",
  "displayName": "Sarah Smith"
}
```
4. Click "Execute"

**Expected Response (200 OK):**
```json
{
  "userId": 1,
  "username": "sarah",
  "email": "sarah@example.com",
  "displayName": "Sarah Smith",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

5. **COPY THE TOKEN!** You'll need it for all other requests.

### 3. Authorize with Your Token

1. Click the **🔒 Authorize** button at the top right of Swagger
2. In the "Value" field, type: `Bearer YOUR_TOKEN_HERE`
   - Replace `YOUR_TOKEN_HERE` with the token you copied
3. Click "Authorize"
4. Click "Close"

✅ You're now authenticated!

### 4. Create Your First Post

1. Find `POST /api/post`
2. Click "Try it out"
3. Enter:
```json
{
  "textContent": "Hello everyone! This is my first post!",
  "type": 0
}
```
4. Click "Execute"

**Expected Response (200 OK):**
```json
{
  "id": 1,
  "userId": 1,
  "username": "sarah",
  "displayName": "Sarah Smith",
  "textContent": "Hello everyone! This is my first post!",
  "type": 0,
  "createdAt": "2024-01-15T10:30:00Z",
  "likesCount": 0,
  "commentsCount": 0,
  "isLikedByCurrentUser": false
}
```

### 5. Create a Post with Image

```json
{
  "textContent": "Check out this cool picture!",
  "type": 1,
  "mediaUrl": "https://picsum.photos/800/600"
}
```

### 6. Get the Feed

1. Find `GET /api/post/feed`
2. Click "Try it out"
3. Click "Execute"

You'll see all posts in the system!

### 7. Add a Comment

1. Find `POST /api/post/comments`
2. Enter:
```json
{
  "postId": 1,
  "content": "Great post! Love it!",
  "parentCommentId": null
}
```
3. Click "Execute"

### 8. Like a Post

1. Find `POST /api/post/{postId}/like`
2. Enter postId: `1`
3. Click "Execute"

**Response:**
```json
{
  "isLiked": true,
  "likesCount": 1
}
```

### 9. Get Notifications

1. Find `GET /api/notification`
2. Click "Execute"

You'll see all your notifications!

## 🔄 Testing Real-time Features

To test SignalR real-time features, you'll need a client. Here's a simple HTML test page:

### Create a Test HTML Page

Create a file called `test-signalr.html`:

```html
<!DOCTYPE html>
<html>
<head>
    <title>SignalR Test</title>
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
</head>
<body>
    <h1>SignalR Post Hub Test</h1>
    
    <div>
        <h3>Your Token:</h3>
        <input id="token" type="text" style="width: 500px" placeholder="Paste your JWT token here">
        <button onclick="connect()">Connect</button>
    </div>
    
    <div id="status">Not connected</div>
    
    <div style="margin-top: 20px;">
        <h3>Join Post:</h3>
        <input id="postId" type="number" value="1">
        <button onclick="joinPost()">Join Post</button>
    </div>
    
    <div style="margin-top: 20px;">
        <h3>Send Comment:</h3>
        <input id="commentText" type="text" placeholder="Your comment" style="width: 300px">
        <button onclick="sendComment()">Send Comment</button>
    </div>
    
    <div style="margin-top: 20px;">
        <h3>Comments Received:</h3>
        <div id="comments"></div>
    </div>

    <script>
        let connection;
        
        async function connect() {
            const token = document.getElementById('token').value;
            
            connection = new signalR.HubConnectionBuilder()
                .withUrl("https://localhost:7001/hubs/post?access_token=" + token)
                .withAutomaticReconnect()
                .build();
            
            // Listen for new comments
            connection.on("ReceiveComment", (comment) => {
                console.log("New comment received:", comment);
                const div = document.getElementById('comments');
                div.innerHTML += `<p><strong>${comment.username}:</strong> ${comment.content}</p>`;
            });
            
            // Listen for likes
            connection.on("PostLikeUpdate", (data) => {
                console.log("Post like update:", data);
                alert(`Post ${data.postId} now has ${data.likesCount} likes!`);
            });
            
            try {
                await connection.start();
                document.getElementById('status').innerHTML = '✅ Connected!';
                console.log("Connected to Post Hub!");
            } catch (err) {
                console.error(err);
                document.getElementById('status').innerHTML = '❌ Connection failed: ' + err;
            }
        }
        
        async function joinPost() {
            const postId = parseInt(document.getElementById('postId').value);
            await connection.invoke("JoinPost", postId);
            alert(`Joined post ${postId}!`);
        }
        
        async function sendComment() {
            const postId = parseInt(document.getElementById('postId').value);
            const content = document.getElementById('commentText').value;
            
            await connection.invoke("SendComment", {
                postId: postId,
                content: content,
                parentCommentId: null
            });
            
            document.getElementById('commentText').value = '';
        }
    </script>
</body>
</html>
```

**How to use:**
1. Open the HTML file in your browser
2. Paste your JWT token in the input field
3. Click "Connect"
4. Enter a post ID and click "Join Post"
5. Type a comment and click "Send Comment"
6. Open the page in another browser tab to see real-time updates!

## 📊 Testing Multiple Users

To test the social features properly:

### Create Multiple Users

```bash
# User 1
POST /api/auth/register
{
  "username": "alice",
  "email": "alice@example.com",
  "password": "Test123!",
  "displayName": "Alice"
}

# User 2
POST /api/auth/register
{
  "username": "bob",
  "email": "bob@example.com",
  "password": "Test123!",
  "displayName": "Bob"
}
```

### Test Scenario

1. **Alice creates a post** (use Alice's token)
   ```
   POST /api/post
   {
     "textContent": "What's everyone's favorite game?",
     "type": 0
   }
   ```

2. **Bob comments on Alice's post** (use Bob's token)
   ```
   POST /api/post/comments
   {
     "postId": 1,
     "content": "I love Minecraft!",
     "parentCommentId": null
   }
   ```

3. **Alice gets a notification** (use Alice's token)
   ```
   GET /api/notification
   ```

4. **Bob likes Alice's post**
   ```
   POST /api/post/1/like
   ```

5. **Alice gets another notification**

## 🔍 Common Testing Endpoints

### Authentication
- `POST /api/auth/register` - Create new user
- `POST /api/auth/login` - Login existing user
- `GET /api/auth/me` - Get current user info

### Posts
- `GET /api/post/feed` - Get all posts (paginated)
- `POST /api/post` - Create post
- `GET /api/post/{id}` - Get specific post
- `PUT /api/post/{id}` - Update post
- `DELETE /api/post/{id}` - Delete post
- `POST /api/post/{id}/like` - Like/unlike post

### Comments
- `GET /api/post/{postId}/comments` - Get comments
- `POST /api/post/comments` - Add comment
- `PUT /api/post/comments/{id}` - Update comment
- `DELETE /api/post/comments/{id}` - Delete comment
- `POST /api/post/comments/{id}/like` - Like/unlike comment

### Direct Messages
- `GET /api/chat/conversations` - Get conversations
- `POST /api/chat/conversations` - Create conversation
- `GET /api/chat/conversations/{id}/messages` - Get messages
- `POST /api/chat/messages` - Send message (or use SignalR)

### Notifications
- `GET /api/notification` - Get notifications
- `GET /api/notification/unread-count` - Get unread count
- `PUT /api/notification/{id}/read` - Mark as read

## ❌ Troubleshooting

### "Connection string error"
```bash
# Check your SQL Server is running
# Update appsettings.json with correct connection string
```

### "Migration already exists"
```bash
# Remove the migration
dotnet ef migrations remove

# Delete Migrations folder
# Try again
dotnet ef migrations add InitialCreate
```

### "Database update failed"
```bash
# Drop the database and recreate
dotnet ef database drop
dotnet ef database update
```

### "Unauthorized 401"
- Make sure you clicked the Authorize button in Swagger
- Check your token hasn't expired (tokens last 7 days)
- Re-login to get a new token

### "Port already in use"
- Stop any other running instances
- Or change the port in `Properties/launchSettings.json`

## 🎯 Next Steps

1. ✅ Test all CRUD operations (Create, Read, Update, Delete)
2. ✅ Test with multiple users
3. ✅ Test real-time features with SignalR
4. ✅ Check notifications work
5. ✅ Test pagination on feed
6. ✅ Build a frontend!

## 📝 Quick Commands Reference

```bash
# Run the app
dotnet run

# Run with hot reload
dotnet watch run

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Drop database
dotnet ef database drop

# Remove last migration
dotnet ef migrations remove

# Check .NET version
dotnet --version

# List all migrations
dotnet ef migrations list
```

---

**Happy Testing! 🎉**


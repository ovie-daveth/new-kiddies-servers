# API Testing Examples (cURL & JavaScript)

Complete examples for testing all endpoints.

## Setup

```bash
# Base URL
BASE_URL="https://localhost:7001"

# Store your token after login
TOKEN="your_jwt_token_here"
```

---

## Authentication Examples

### 1. Register a User

**cURL:**
```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "sarah123",
    "email": "sarah@example.com",
    "password": "Test123!",
    "displayName": "Sarah Smith"
  }'
```

**JavaScript (Fetch):**
```javascript
const response = await fetch('https://localhost:7001/api/auth/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'sarah123',
    email: 'sarah@example.com',
    password: 'Test123!',
    displayName: 'Sarah Smith'
  })
});
const { token } = await response.json();
console.log('Token:', token);
```

### 2. Login

**cURL:**
```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "sarah@example.com",
    "password": "Test123!"
  }'
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'sarah@example.com',
    password: 'Test123!'
  })
});
const { token, userId } = await response.json();
localStorage.setItem('authToken', token);
```

### 3. Get Current User

**cURL:**
```bash
curl -X GET https://localhost:7001/api/auth/me \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/auth/me', {
  headers: { 'Authorization': `Bearer ${token}` }
});
const user = await response.json();
```

### 4. Search Users

**cURL:**
```bash
curl -X GET "https://localhost:7001/api/auth/users/search?query=sarah" \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const query = 'sarah';
const response = await fetch(
  `https://localhost:7001/api/auth/users/search?query=${encodeURIComponent(query)}`,
  { headers: { 'Authorization': `Bearer ${token}` } }
);
const users = await response.json();
```

---

## Post Examples

### 1. Create Text Post

**cURL:**
```bash
curl -X POST https://localhost:7001/api/post \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "textContent": "Hello world! My first post!",
    "type": 0
  }'
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/post', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    textContent: 'Hello world! My first post!',
    type: 0
  })
});
const post = await response.json();
```

### 2. Create Image Post

**cURL:**
```bash
curl -X POST https://localhost:7001/api/post \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "textContent": "Check out this cool picture!",
    "type": 1,
    "mediaUrl": "https://picsum.photos/800/600"
  }'
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/post', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    textContent: 'Check out this cool picture!',
    type: 1,
    mediaUrl: 'https://picsum.photos/800/600'
  })
});
const post = await response.json();
```

### 3. Create Video Post

**cURL:**
```bash
curl -X POST https://localhost:7001/api/post \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "textContent": "My dance video!",
    "type": 2,
    "mediaUrl": "https://example.com/video.mp4",
    "thumbnailUrl": "https://example.com/thumb.jpg"
  }'
```

### 4. Get Feed (Paginated)

**cURL:**
```bash
curl -X GET "https://localhost:7001/api/post/feed?skip=0&take=20" \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const skip = 0;
const take = 20;
const response = await fetch(
  `https://localhost:7001/api/post/feed?skip=${skip}&take=${take}`,
  { headers: { 'Authorization': `Bearer ${token}` } }
);
const { posts, totalCount, hasMore } = await response.json();
```

### 5. Get Specific Post

**cURL:**
```bash
curl -X GET https://localhost:7001/api/post/5 \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const postId = 5;
const response = await fetch(`https://localhost:7001/api/post/${postId}`, {
  headers: { 'Authorization': `Bearer ${token}` }
});
const post = await response.json();
```

### 6. Get User's Posts

**cURL:**
```bash
curl -X GET "https://localhost:7001/api/post/user/1?skip=0&take=20" \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const userId = 1;
const response = await fetch(
  `https://localhost:7001/api/post/user/${userId}?skip=0&take=20`,
  { headers: { 'Authorization': `Bearer ${token}` } }
);
const { posts, totalCount, hasMore } = await response.json();
```

### 7. Update Post

**cURL:**
```bash
curl -X PUT https://localhost:7001/api/post/5 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "textContent": "Updated: Hello world!"
  }'
```

**JavaScript:**
```javascript
const postId = 5;
const response = await fetch(`https://localhost:7001/api/post/${postId}`, {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    textContent: 'Updated: Hello world!'
  })
});
const updatedPost = await response.json();
```

### 8. Delete Post

**cURL:**
```bash
curl -X DELETE https://localhost:7001/api/post/5 \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const postId = 5;
await fetch(`https://localhost:7001/api/post/${postId}`, {
  method: 'DELETE',
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### 9. Like/Unlike Post

**cURL:**
```bash
curl -X POST https://localhost:7001/api/post/5/like \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const postId = 5;
const response = await fetch(`https://localhost:7001/api/post/${postId}/like`, {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` }
});
const { isLiked, likesCount } = await response.json();
console.log(`Post ${isLiked ? 'liked' : 'unliked'}. Total likes: ${likesCount}`);
```

---

## Comment Examples

### 1. Add Top-Level Comment to Post

**IMPORTANT:** For first comments on a post, `parentCommentId` MUST be `null`

**cURL:**
```bash
curl -X POST https://localhost:7001/api/post/comments \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "postId": 5,
    "content": "Great post! Love it!",
    "parentCommentId": null
  }'
```

**❌ WRONG - Don't do this:**
```bash
# This will fail if comment 1 doesn't exist!
curl -X POST https://localhost:7001/api/post/comments \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "postId": 5,
    "content": "Great post!",
    "parentCommentId": 1    # ← ERROR! Use null for first comment
  }'
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/post/comments', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    postId: 5,
    content: 'Great post! Love it!',
    parentCommentId: null
  })
});
const comment = await response.json();
```

### 2. Reply to Comment

**cURL:**
```bash
curl -X POST https://localhost:7001/api/post/comments \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "postId": 5,
    "content": "Thanks so much!",
    "parentCommentId": 1
  }'
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/post/comments', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    postId: 5,
    content: 'Thanks so much!',
    parentCommentId: 1  // Replying to comment ID 1
  })
});
const reply = await response.json();
```

### 3. Get Comments for Post

**cURL:**
```bash
curl -X GET "https://localhost:7001/api/post/5/comments?skip=0&take=50" \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const postId = 5;
const response = await fetch(
  `https://localhost:7001/api/post/${postId}/comments?skip=0&take=50`,
  { headers: { 'Authorization': `Bearer ${token}` } }
);
const comments = await response.json();
// Comments include nested replies
```

### 4. Update Comment

**cURL:**
```bash
curl -X PUT https://localhost:7001/api/post/comments/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "content": "Updated comment text"
  }'
```

**JavaScript:**
```javascript
const commentId = 1;
const response = await fetch(`https://localhost:7001/api/post/comments/${commentId}`, {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    content: 'Updated comment text'
  })
});
const updatedComment = await response.json();
```

### 5. Delete Comment

**cURL:**
```bash
curl -X DELETE https://localhost:7001/api/post/comments/1 \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const commentId = 1;
await fetch(`https://localhost:7001/api/post/comments/${commentId}`, {
  method: 'DELETE',
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### 6. Like/Unlike Comment

**cURL:**
```bash
curl -X POST https://localhost:7001/api/post/comments/1/like \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const commentId = 1;
const response = await fetch(`https://localhost:7001/api/post/comments/${commentId}/like`, {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` }
});
const { isLiked, likesCount } = await response.json();
```

---

## Direct Messaging Examples

### 1. Create Direct Message Conversation

**cURL:**
```bash
curl -X POST https://localhost:7001/api/chat/conversations \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "participantIds": [5],
    "isGroup": false
  }'
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/chat/conversations', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    participantIds: [5],  // User ID to chat with
    isGroup: false
  })
});
const conversation = await response.json();
```

### 2. Create Group Chat

**cURL:**
```bash
curl -X POST https://localhost:7001/api/chat/conversations \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "participantIds": [2, 3, 4],
    "name": "Study Group",
    "isGroup": true
  }'
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/chat/conversations', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    participantIds: [2, 3, 4],
    name: 'Study Group',
    isGroup: true
  })
});
const groupChat = await response.json();
```

### 3. Get All Conversations

**cURL:**
```bash
curl -X GET https://localhost:7001/api/chat/conversations \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/chat/conversations', {
  headers: { 'Authorization': `Bearer ${token}` }
});
const conversations = await response.json();
```

### 4. Get Conversation Messages

**cURL:**
```bash
curl -X GET "https://localhost:7001/api/chat/conversations/1/messages?skip=0&take=50" \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const conversationId = 1;
const response = await fetch(
  `https://localhost:7001/api/chat/conversations/${conversationId}/messages?skip=0&take=50`,
  { headers: { 'Authorization': `Bearer ${token}` } }
);
const messages = await response.json();
```

### 5. Send Message

**cURL:**
```bash
curl -X POST https://localhost:7001/api/chat/messages \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "conversationId": 1,
    "content": "Hello everyone!",
    "type": 0
  }'
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/chat/messages', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    conversationId: 1,
    content: 'Hello everyone!',
    type: 0  // 0=Text, 1=Image, 2=File, 3=System
  })
});
const message = await response.json();
```

---

## Notification Examples

### 1. Get Notifications

**cURL:**
```bash
curl -X GET "https://localhost:7001/api/notification?skip=0&take=20" \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const response = await fetch(
  'https://localhost:7001/api/notification?skip=0&take=20',
  { headers: { 'Authorization': `Bearer ${token}` } }
);
const notifications = await response.json();
```

### 2. Get Unread Count

**cURL:**
```bash
curl -X GET https://localhost:7001/api/notification/unread-count \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const response = await fetch('https://localhost:7001/api/notification/unread-count', {
  headers: { 'Authorization': `Bearer ${token}` }
});
const unreadCount = await response.json();
console.log(`You have ${unreadCount} unread notifications`);
```

### 3. Mark Notification as Read

**cURL:**
```bash
curl -X PUT https://localhost:7001/api/notification/1/read \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
const notificationId = 1;
await fetch(`https://localhost:7001/api/notification/${notificationId}/read`, {
  method: 'PUT',
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### 4. Mark All as Read

**cURL:**
```bash
curl -X PUT https://localhost:7001/api/notification/mark-all-read \
  -H "Authorization: Bearer $TOKEN"
```

**JavaScript:**
```javascript
await fetch('https://localhost:7001/api/notification/mark-all-read', {
  method: 'PUT',
  headers: { 'Authorization': `Bearer ${token}` }
});
```

---

## SignalR Examples

### Post Hub - Real-time Comments & Likes

```javascript
import * as signalR from "@microsoft/signalr";

// Connect to Post Hub
const postHub = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/post?access_token=" + token)
    .withAutomaticReconnect()
    .build();

// Listen for new comments
postHub.on("ReceiveComment", (comment) => {
    console.log("New comment received:", comment);
    // Update UI with new comment
});

// Listen for like updates
postHub.on("PostLikeUpdate", (data) => {
    console.log(`Post ${data.postId} now has ${data.likesCount} likes`);
    // Update like count in UI
});

// Start connection
await postHub.start();
console.log("Connected to Post Hub");

// Join a specific post to receive its updates
const postId = 5;
await postHub.invoke("JoinPost", postId);

// Send a comment
await postHub.invoke("SendComment", {
    postId: 5,
    content: "Great post!",
    parentCommentId: null
});

// Like a post
await postHub.invoke("LikePost", 5);

// Show typing indicator
await postHub.invoke("UserTypingComment", 5, true);
// Stop typing
await postHub.invoke("UserTypingComment", 5, false);
```

### Chat Hub - Real-time Messaging

```javascript
// Connect to Chat Hub
const chatHub = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/chat?access_token=" + token)
    .withAutomaticReconnect()
    .build();

// Listen for messages
chatHub.on("ReceiveMessage", (message) => {
    console.log("New message:", message);
    // Display message in chat
});

// Listen for user online/offline
chatHub.on("UserOnline", (userId) => {
    console.log(`User ${userId} is now online`);
});

chatHub.on("UserOffline", (userId) => {
    console.log(`User ${userId} is now offline`);
});

// Listen for typing indicator
chatHub.on("UserTyping", (userId, conversationId, isTyping) => {
    console.log(`User ${userId} is ${isTyping ? 'typing' : 'stopped typing'}`);
});

// Start connection
await chatHub.start();
console.log("Connected to Chat Hub");

// Join a conversation
const conversationId = 1;
await chatHub.invoke("JoinConversation", conversationId);

// Send a message
await chatHub.invoke("SendMessage", {
    conversationId: 1,
    content: "Hello!",
    type: 0
});

// Show typing indicator
await chatHub.invoke("TypingIndicator", 1, true);

// Mark message as read
await chatHub.invoke("MarkMessageAsRead", 10);
```

### Notification Hub - Real-time Notifications

```javascript
// Connect to Notification Hub
const notificationHub = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/notification?access_token=" + token)
    .withAutomaticReconnect()
    .build();

// Listen for new notifications
notificationHub.on("ReceiveNotification", (notification) => {
    console.log("New notification:", notification);
    // Show notification toast/banner
    showNotification(notification.title, notification.message);
});

// Listen for unread count updates
notificationHub.on("UnreadNotificationsCount", (count) => {
    console.log(`Unread count: ${count}`);
    // Update notification badge
    updateBadge(count);
});

// Start connection
await notificationHub.start();
console.log("Connected to Notification Hub");

// Mark notification as read
await notificationHub.invoke("MarkNotificationAsRead", 5);

// Mark all as read
await notificationHub.invoke("MarkAllAsRead");
```

---

## Complete Testing Workflow

```bash
#!/bin/bash
# Complete test workflow script

BASE_URL="https://localhost:7001"

# 1. Register User 1
echo "=== Registering User 1 ==="
USER1_RESPONSE=$(curl -s -X POST $BASE_URL/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "alice",
    "email": "alice@example.com",
    "password": "Test123!",
    "displayName": "Alice Wonder"
  }')

USER1_TOKEN=$(echo $USER1_RESPONSE | jq -r '.token')
echo "User 1 Token: $USER1_TOKEN"

# 2. Register User 2
echo "=== Registering User 2 ==="
USER2_RESPONSE=$(curl -s -X POST $BASE_URL/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "bob",
    "email": "bob@example.com",
    "password": "Test123!",
    "displayName": "Bob Builder"
  }')

USER2_TOKEN=$(echo $USER2_RESPONSE | jq -r '.token')
echo "User 2 Token: $USER2_TOKEN"

# 3. User 1 creates a post
echo "=== User 1 Creating Post ==="
POST_RESPONSE=$(curl -s -X POST $BASE_URL/api/post \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $USER1_TOKEN" \
  -d '{
    "textContent": "Hello world from Alice!",
    "type": 0
  }')

POST_ID=$(echo $POST_RESPONSE | jq -r '.id')
echo "Post ID: $POST_ID"

# 4. User 2 comments on the post
echo "=== User 2 Commenting ==="
curl -s -X POST $BASE_URL/api/post/comments \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $USER2_TOKEN" \
  -d "{
    \"postId\": $POST_ID,
    \"content\": \"Great post Alice!\",
    \"parentCommentId\": null
  }" | jq

# 5. User 2 likes the post
echo "=== User 2 Liking Post ==="
curl -s -X POST $BASE_URL/api/post/$POST_ID/like \
  -H "Authorization: Bearer $USER2_TOKEN" | jq

# 6. Get feed
echo "=== Getting Feed ==="
curl -s -X GET "$BASE_URL/api/post/feed?skip=0&take=10" \
  -H "Authorization: Bearer $USER1_TOKEN" | jq

# 7. User 1 checks notifications
echo "=== User 1 Notifications ==="
curl -s -X GET $BASE_URL/api/notification \
  -H "Authorization: Bearer $USER1_TOKEN" | jq

echo "=== Test Complete ==="
```

---

## Error Handling Example

```javascript
async function createPost(token, postData) {
  try {
    const response = await fetch('https://localhost:7001/api/post', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(postData)
    });

    if (!response.ok) {
      // Handle HTTP errors
      if (response.status === 401) {
        throw new Error('Unauthorized - please login again');
      } else if (response.status === 400) {
        const error = await response.json();
        throw new Error(`Bad request: ${error.message}`);
      } else {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }
    }

    return await response.json();
  } catch (error) {
    console.error('Error creating post:', error);
    throw error;
  }
}

// Usage
try {
  const post = await createPost(token, {
    textContent: 'My post',
    type: 0
  });
  console.log('Post created:', post);
} catch (error) {
  alert(`Failed to create post: ${error.message}`);
}
```

---

For detailed API documentation, see [API_DOCUMENTATION.md](API_DOCUMENTATION.md)  
For quick reference, see [API_QUICK_REFERENCE.md](API_QUICK_REFERENCE.md)


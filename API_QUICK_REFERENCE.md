# API Quick Reference Guide

Quick lookup table for all endpoints.

## Authentication Endpoints

| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| POST | `/api/auth/register` | ❌ | Register new user |
| POST | `/api/auth/login` | ❌ | Login user |
| GET | `/api/auth/me` | ✅ | Get current user |
| GET | `/api/auth/users/search?query={q}` | ✅ | Search users |
| GET | `/api/auth/users/{userId}` | ✅ | Get user by ID |

## Post Endpoints

| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| POST | `/api/post` | ✅ | Create new post |
| GET | `/api/post/feed?skip={s}&take={t}` | ✅ | Get feed (paginated) |
| GET | `/api/post/{postId}` | ✅ | Get specific post |
| GET | `/api/post/user/{userId}?skip={s}&take={t}` | ✅ | Get user's posts |
| PUT | `/api/post/{postId}` | ✅ | Update post (owner only) |
| DELETE | `/api/post/{postId}` | ✅ | Delete post (owner only) |
| POST | `/api/post/{postId}/like` | ✅ | Toggle like on post |

## Comment Endpoints

| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| POST | `/api/post/comments` | ✅ | Add comment/reply |
| GET | `/api/post/{postId}/comments?skip={s}&take={t}` | ✅ | Get post comments |
| PUT | `/api/post/comments/{commentId}` | ✅ | Update comment (owner) |
| DELETE | `/api/post/comments/{commentId}` | ✅ | Delete comment (owner) |
| POST | `/api/post/comments/{commentId}/like` | ✅ | Toggle like on comment |

## Chat/Messaging Endpoints

| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| POST | `/api/chat/conversations` | ✅ | Create conversation |
| GET | `/api/chat/conversations` | ✅ | Get all conversations |
| GET | `/api/chat/conversations/{id}` | ✅ | Get conversation details |
| GET | `/api/chat/conversations/{id}/messages?skip={s}&take={t}` | ✅ | Get messages |
| POST | `/api/chat/messages` | ✅ | Send message |

## Notification Endpoints

| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| GET | `/api/notification?skip={s}&take={t}` | ✅ | Get notifications |
| GET | `/api/notification/unread-count` | ✅ | Get unread count |
| PUT | `/api/notification/{id}/read` | ✅ | Mark as read |
| PUT | `/api/notification/mark-all-read` | ✅ | Mark all as read |

---

## Common Request Bodies

### Register User
```json
{
  "username": "sarah123",
  "email": "sarah@example.com",
  "password": "SecurePass123!",
  "displayName": "Sarah Smith"
}
```

### Login User
```json
{
  "email": "sarah@example.com",
  "password": "SecurePass123!"
}
```

### Create Post
```json
{
  "textContent": "Hello world!",
  "type": 0,
  "mediaUrl": "https://example.com/image.jpg",
  "thumbnailUrl": "https://example.com/thumb.jpg"
}
```

**Post Types:** 0=Text, 1=Image, 2=Video, 3=TextWithImage, 4=TextWithVideo

### Add Comment
**Top-level comment (use null):**
```json
{
  "postId": 5,
  "content": "Great post!",
  "parentCommentId": null    // ← MUST be null for first comment
}
```

**Reply to comment:**
```json
{
  "postId": 5,
  "content": "I agree!",
  "parentCommentId": 10    // ← Use actual comment ID
}
```

### Create Conversation (DM)
```json
{
  "participantIds": [5],
  "isGroup": false
}
```

### Create Conversation (Group)
```json
{
  "participantIds": [2, 3, 4],
  "name": "Study Group",
  "isGroup": true
}
```

### Send Message
```json
{
  "conversationId": 1,
  "content": "Hello!",
  "type": 0
}
```

**Message Types:** 0=Text, 1=Image, 2=File, 3=System

---

## SignalR Hubs

### Post Hub - `/hubs/post`

**Send Events:**
- `JoinPost(postId)` - Join post room
- `LeavePost(postId)` - Leave post room
- `SendComment(commentDto)` - Post comment
- `LikePost(postId)` - Like/unlike post
- `LikeComment(commentId)` - Like/unlike comment
- `UserTypingComment(postId, isTyping)` - Typing indicator

**Receive Events:**
- `ReceiveComment(comment)` - New comment
- `NewComment({ postId, commentCount })` - Comment count update
- `PostLikeUpdate({ postId, likesCount, isLiked, userId })` - Like update
- `CommentLikeUpdate({ commentId, likesCount, isLiked, userId })` - Comment like
- `UserTypingComment(userId, postId, isTyping)` - User typing

### Chat Hub - `/hubs/chat`

**Send Events:**
- `SendMessage(messageDto)` - Send message
- `JoinConversation(conversationId)` - Join conversation
- `LeaveConversation(conversationId)` - Leave conversation
- `TypingIndicator(conversationId, isTyping)` - Typing status
- `MarkMessageAsRead(messageId)` - Mark message read

**Receive Events:**
- `ReceiveMessage(message)` - New message
- `MessageSent(message)` - Message sent confirmation
- `UserOnline(userId)` - User online
- `UserOffline(userId)` - User offline
- `UserJoinedConversation(userId, conversationId)` - User joined
- `UserLeftConversation(userId, conversationId)` - User left
- `UserTyping(userId, conversationId, isTyping)` - User typing
- `MessageRead(messageId, userId)` - Message read

### Notification Hub - `/hubs/notification`

**Send Events:**
- `MarkNotificationAsRead(notificationId)` - Mark as read
- `MarkAllAsRead()` - Mark all read

**Receive Events:**
- `ReceiveNotification(notification)` - New notification
- `UnreadNotificationsCount(count)` - Unread count

---

## Response Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 204 | No Content (successful deletion/update) |
| 400 | Bad Request (invalid input) |
| 401 | Unauthorized (missing/invalid token) |
| 403 | Forbidden (not authorized for action) |
| 404 | Not Found |
| 500 | Internal Server Error |

---

## Field Constraints

| Field | Min | Max | Notes |
|-------|-----|-----|-------|
| Username | 3 | 50 | Alphanumeric + underscore |
| Email | - | 100 | Valid email format |
| Password | 6 | - | Strong password recommended |
| DisplayName | - | 100 | Optional |
| Post Text | - | 5000 | Optional if media present |
| Comment | 1 | 2000 | Required |
| Message | 1 | 4000 | Required |
| Media URL | - | 500 | Valid URL |
| Group Name | - | 100 | Required for groups |

---

## Authentication Header

All authenticated endpoints require:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Token expires after **7 days**.

---

## Pagination Defaults

| Endpoint | Default Skip | Default Take | Max Take |
|----------|-------------|--------------|----------|
| Post Feed | 0 | 20 | 100 |
| User Posts | 0 | 20 | 100 |
| Comments | 0 | 50 | 100 |
| Messages | 0 | 50 | 100 |
| Notifications | 0 | 20 | 100 |

---

## SignalR Connection Example

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/post?access_token=" + token)
    .withAutomaticReconnect()
    .build();

await connection.start();
```

---

For detailed documentation, see [API_DOCUMENTATION.md](API_DOCUMENTATION.md)


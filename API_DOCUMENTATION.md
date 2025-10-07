# API Documentation - Kids Social Media Platform

## Table of Contents
1. [Authentication](#authentication)
2. [Posts](#posts)
3. [Comments](#comments)
4. [Likes](#likes)
5. [Direct Messaging](#direct-messaging)
6. [Notifications](#notifications)
7. [Users](#users)
8. [Error Codes](#error-codes)
9. [Data Models](#data-models)

---

## Base URL
- **Development:** `https://localhost:7001`
- **Production:** `https://yourdomain.com`

## Authentication

All endpoints except `register` and `login` require authentication via JWT Bearer token.

**How to authenticate:**
```
Authorization: Bearer {your_jwt_token}
```

---

### POST /api/auth/register

**Purpose:** Create a new user account

**Authentication:** None required

**Request Body:**
```json
{
  "username": "string",      // Required, 3-50 characters, unique
  "email": "string",          // Required, valid email format, unique
  "password": "string",       // Required, minimum 6 characters
  "displayName": "string"     // Optional, friendly name shown to others
}
```

**Constraints:**
- Username: Must be unique, 3-50 characters, alphanumeric and underscores only
- Email: Must be unique and valid email format
- Password: Minimum 6 characters (recommend strong password with mix of characters)
- DisplayName: Optional, max 100 characters

**Success Response (200 OK):**
```json
{
  "userId": 1,
  "username": "sarah123",
  "email": "sarah@example.com",
  "displayName": "Sarah Smith",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Responses:**
- **400 Bad Request:** Invalid input, duplicate username/email
  ```json
  {
    "message": "User with this email already exists"
  }
  ```

**Use Case:**
- New user signs up to the platform
- Returns JWT token for immediate authentication

---

### POST /api/auth/login

**Purpose:** Authenticate existing user and get access token

**Authentication:** None required

**Request Body:**
```json
{
  "email": "string",      // Required
  "password": "string"     // Required
}
```

**Success Response (200 OK):**
```json
{
  "userId": 1,
  "username": "sarah123",
  "email": "sarah@example.com",
  "displayName": "Sarah Smith",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Responses:**
- **400 Bad Request:** Invalid credentials
  ```json
  {
    "message": "Invalid email or password"
  }
  ```

**Use Case:**
- User logs in to access their account
- Token expires after 7 days

**Token Details:**
- Expiration: 7 days from issue
- Contains: userId, username, email
- Algorithm: HS256

---

### GET /api/auth/me

**Purpose:** Get current authenticated user's information

**Authentication:** Required

**Success Response (200 OK):**
```json
{
  "id": 1,
  "username": "sarah123",
  "displayName": "Sarah Smith",
  "profilePictureUrl": "https://example.com/avatar.jpg",
  "isOnline": true,
  "lastSeen": "2024-01-15T10:30:00Z"
}
```

**Error Responses:**
- **401 Unauthorized:** No token or invalid token
- **404 Not Found:** User not found

**Use Case:**
- Verify token is still valid
- Get current user profile data
- Check authentication status

---

### GET /api/auth/users/search

**Purpose:** Search for users by username or display name

**Authentication:** Required

**Query Parameters:**
- `query` (required): Search term, minimum 1 character

**Example Request:**
```
GET /api/auth/users/search?query=sarah
```

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "username": "sarah123",
    "displayName": "Sarah Smith",
    "profilePictureUrl": "https://example.com/avatar.jpg",
    "isOnline": true,
    "lastSeen": "2024-01-15T10:30:00Z"
  },
  {
    "id": 2,
    "username": "sarah_cool",
    "displayName": "Sarah Johnson",
    "profilePictureUrl": null,
    "isOnline": false,
    "lastSeen": "2024-01-14T15:20:00Z"
  }
]
```

**Constraints:**
- Returns maximum 20 results
- Searches both username and displayName fields
- Case-insensitive search
- Partial match supported

**Error Responses:**
- **400 Bad Request:** Missing or empty query parameter

**Use Case:**
- Find friends to message
- Search for specific users
- Discover new users

---

### GET /api/auth/users/{userId}

**Purpose:** Get public profile information for a specific user

**Authentication:** Required

**Path Parameters:**
- `userId` (required): Integer, user ID

**Example Request:**
```
GET /api/auth/users/5
```

**Success Response (200 OK):**
```json
{
  "id": 5,
  "username": "john_doe",
  "displayName": "John Doe",
  "profilePictureUrl": "https://example.com/john.jpg",
  "isOnline": false,
  "lastSeen": "2024-01-15T08:45:00Z"
}
```

**Error Responses:**
- **404 Not Found:** User doesn't exist

**Use Case:**
- View user profile
- Check if user is online
- Get user information for display

---

## Posts

### POST /api/post

**Purpose:** Create a new post with text, image, or video content

**Authentication:** Required

**Request Body:**
```json
{
  "textContent": "string",     // Optional, max 5000 characters
  "type": 0,                   // Required: 0=Text, 1=Image, 2=Video, 3=TextWithImage, 4=TextWithVideo
  "mediaUrl": "string",        // Optional, URL to image/video, max 500 chars
  "thumbnailUrl": "string"     // Optional, URL to video thumbnail, max 500 chars
}
```

**Post Types:**
- `0` - **Text**: Text-only post
- `1` - **Image**: Image with optional text
- `2` - **Video**: Video with optional text
- `3` - **TextWithImage**: Text and image
- `4` - **TextWithVideo**: Text and video

**Constraints:**
- At least textContent OR mediaUrl must be provided
- textContent: Maximum 5000 characters
- mediaUrl: Maximum 500 characters, must be valid URL
- thumbnailUrl: Maximum 500 characters, only for video posts

**Success Response (200 OK):**
```json
{
  "id": 1,
  "userId": 1,
  "username": "sarah123",
  "displayName": "Sarah Smith",
  "profilePictureUrl": "https://example.com/avatar.jpg",
  "textContent": "Check out this amazing sunset!",
  "type": 1,
  "mediaUrl": "https://example.com/sunset.jpg",
  "thumbnailUrl": null,
  "createdAt": "2024-01-15T10:30:00Z",
  "editedAt": null,
  "isEdited": false,
  "likesCount": 0,
  "commentsCount": 0,
  "isLikedByCurrentUser": false
}
```

**Error Responses:**
- **400 Bad Request:** Invalid data, missing required fields

**Use Case:**
- Share thoughts, photos, videos
- Create content for the feed
- Express creativity

**Examples:**

**Text Post:**
```json
{
  "textContent": "Having a great day at the park!",
  "type": 0
}
```

**Image Post:**
```json
{
  "textContent": "Look at my drawing!",
  "type": 1,
  "mediaUrl": "https://storage.example.com/drawings/pic123.jpg"
}
```

**Video Post:**
```json
{
  "textContent": "My dance performance!",
  "type": 2,
  "mediaUrl": "https://storage.example.com/videos/dance.mp4",
  "thumbnailUrl": "https://storage.example.com/thumbs/dance.jpg"
}
```

---

### GET /api/post/feed

**Purpose:** Get paginated feed of all posts from all users

**Authentication:** Required

**Query Parameters:**
- `skip` (optional): Number of posts to skip, default 0
- `take` (optional): Number of posts to return, default 20, max 100

**Example Request:**
```
GET /api/post/feed?skip=0&take=20
```

**Success Response (200 OK):**
```json
{
  "posts": [
    {
      "id": 10,
      "userId": 5,
      "username": "john_doe",
      "displayName": "John Doe",
      "profilePictureUrl": "https://example.com/john.jpg",
      "textContent": "Great day at school!",
      "type": 0,
      "mediaUrl": null,
      "thumbnailUrl": null,
      "createdAt": "2024-01-15T14:30:00Z",
      "editedAt": null,
      "isEdited": false,
      "likesCount": 5,
      "commentsCount": 3,
      "isLikedByCurrentUser": true
    }
  ],
  "totalCount": 150,
  "hasMore": true
}
```

**Constraints:**
- Posts ordered by newest first (createdAt DESC)
- Maximum 100 posts per request
- Deleted posts are not returned
- `isLikedByCurrentUser` reflects current user's like status

**Use Case:**
- Main feed/timeline
- Infinite scroll implementation
- Browse all content

**Pagination Example:**
```
// First page
GET /api/post/feed?skip=0&take=20

// Second page
GET /api/post/feed?skip=20&take=20

// Third page
GET /api/post/feed?skip=40&take=20
```

---

### GET /api/post/{postId}

**Purpose:** Get a specific post by ID

**Authentication:** Required

**Path Parameters:**
- `postId` (required): Integer, post ID

**Example Request:**
```
GET /api/post/5
```

**Success Response (200 OK):**
```json
{
  "id": 5,
  "userId": 3,
  "username": "alice",
  "displayName": "Alice Wonder",
  "profilePictureUrl": "https://example.com/alice.jpg",
  "textContent": "My new artwork!",
  "type": 1,
  "mediaUrl": "https://example.com/art.jpg",
  "thumbnailUrl": null,
  "createdAt": "2024-01-15T10:00:00Z",
  "editedAt": null,
  "isEdited": false,
  "likesCount": 15,
  "commentsCount": 8,
  "isLikedByCurrentUser": false
}
```

**Error Responses:**
- **404 Not Found:** Post doesn't exist or was deleted

**Use Case:**
- View single post detail
- Deep linking to specific post
- Share post URL

---

### GET /api/post/user/{userId}

**Purpose:** Get all posts from a specific user

**Authentication:** Required

**Path Parameters:**
- `userId` (required): Integer, user ID

**Query Parameters:**
- `skip` (optional): Number of posts to skip, default 0
- `take` (optional): Number of posts to return, default 20

**Example Request:**
```
GET /api/post/user/5?skip=0&take=20
```

**Success Response (200 OK):**
```json
{
  "posts": [
    {
      "id": 25,
      "userId": 5,
      "username": "john_doe",
      "displayName": "John Doe",
      "profilePictureUrl": "https://example.com/john.jpg",
      "textContent": "My latest creation",
      "type": 0,
      "mediaUrl": null,
      "thumbnailUrl": null,
      "createdAt": "2024-01-15T12:00:00Z",
      "editedAt": null,
      "isEdited": false,
      "likesCount": 3,
      "commentsCount": 1,
      "isLikedByCurrentUser": false
    }
  ],
  "totalCount": 25,
  "hasMore": true
}
```

**Use Case:**
- User profile page showing their posts
- View all content from specific user
- User portfolio

---

### PUT /api/post/{postId}

**Purpose:** Update/edit an existing post (owner only)

**Authentication:** Required

**Path Parameters:**
- `postId` (required): Integer, post ID

**Request Body:**
```json
{
  "textContent": "string"     // Optional, new text content
}
```

**Constraints:**
- Only the post owner can update
- Can only update textContent
- Cannot change mediaUrl or type
- Maximum 5000 characters

**Success Response (200 OK):**
```json
{
  "id": 5,
  "userId": 1,
  "username": "sarah123",
  "displayName": "Sarah Smith",
  "profilePictureUrl": "https://example.com/avatar.jpg",
  "textContent": "Updated: Check out this amazing sunset!",
  "type": 1,
  "mediaUrl": "https://example.com/sunset.jpg",
  "thumbnailUrl": null,
  "createdAt": "2024-01-15T10:30:00Z",
  "editedAt": "2024-01-15T11:45:00Z",
  "isEdited": true,
  "likesCount": 10,
  "commentsCount": 5,
  "isLikedByCurrentUser": true
}
```

**Error Responses:**
- **403 Forbidden:** Not the post owner
- **404 Not Found:** Post doesn't exist

**Use Case:**
- Fix typos
- Update post content
- Clarify information

---

### DELETE /api/post/{postId}

**Purpose:** Delete a post (owner only)

**Authentication:** Required

**Path Parameters:**
- `postId` (required): Integer, post ID

**Constraints:**
- Only the post owner can delete
- Soft delete (marked as deleted, not removed from database)
- Associated comments and likes remain in database but post won't appear in feeds

**Success Response (204 No Content):**
No response body

**Error Responses:**
- **403 Forbidden:** Not the post owner
- **404 Not Found:** Post doesn't exist

**Use Case:**
- Remove unwanted content
- Delete inappropriate posts
- Clean up profile

---

## Comments

### POST /api/post/comments

**Purpose:** Add a comment to a post or reply to another comment

**Authentication:** Required

**Request Body:**
```json
{
  "postId": 5,                 // Required, post to comment on
  "content": "string",         // Required, max 2000 characters
  "parentCommentId": null      // Optional: null for top-level, comment ID for replies
}
```

**IMPORTANT:** 
- For **first-time comments** on a post, use `parentCommentId: null`
- For **replies** to comments, use `parentCommentId: <comment_id>`
- DO NOT use `parentCommentId: 1` for first comment - this will cause an error!

**Constraints:**
- content: Required, 1-2000 characters
- postId: Must be valid existing post
- parentCommentId: Optional, for replies to comments

**Success Response (200 OK):**
```json
{
  "id": 1,
  "postId": 5,
  "userId": 2,
  "username": "bob",
  "displayName": "Bob Builder",
  "profilePictureUrl": "https://example.com/bob.jpg",
  "parentCommentId": null,
  "content": "This is amazing! Great work!",
  "createdAt": "2024-01-15T11:00:00Z",
  "editedAt": null,
  "isEdited": false,
  "likesCount": 0,
  "isLikedByCurrentUser": false,
  "replies": []
}
```

**Error Responses:**
- **400 Bad Request:** Invalid data, content too long, parent comment not found
  ```json
  {
    "message": "Parent comment not found"
  }
  ```
  ```json
  {
    "message": "Parent comment does not belong to this post"
  }
  ```
- **404 Not Found:** Post doesn't exist

**Use Case:**
- Engage with posts
- Start discussions
- Reply to other comments

**Examples:**

**Top-level comment (first comment on post):**
```json
{
  "postId": 5,
  "content": "Love this!",
  "parentCommentId": null    // ← MUST be null for first-level comments
}
```

**Reply to comment (nested):**
```json
{
  "postId": 5,
  "content": "Thanks! I appreciate it!",
  "parentCommentId": 10    // ← Use actual comment ID you're replying to
}
```

**Common Mistake (DON'T DO THIS):**
```json
{
  "postId": 5,
  "content": "First comment",
  "parentCommentId": 1    // ❌ WRONG! Will cause error if comment 1 doesn't exist
}
```

---

### GET /api/post/{postId}/comments

**Purpose:** Get all comments for a specific post

**Authentication:** Required

**Path Parameters:**
- `postId` (required): Integer, post ID

**Query Parameters:**
- `skip` (optional): Number of comments to skip, default 0
- `take` (optional): Number of comments to return, default 50

**Example Request:**
```
GET /api/post/5/comments?skip=0&take=50
```

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "postId": 5,
    "userId": 2,
    "username": "bob",
    "displayName": "Bob Builder",
    "profilePictureUrl": "https://example.com/bob.jpg",
    "parentCommentId": null,
    "content": "This is amazing!",
    "createdAt": "2024-01-15T11:00:00Z",
    "editedAt": null,
    "isEdited": false,
    "likesCount": 3,
    "isLikedByCurrentUser": true,
    "replies": [
      {
        "id": 2,
        "postId": 5,
        "userId": 1,
        "username": "sarah123",
        "displayName": "Sarah Smith",
        "profilePictureUrl": "https://example.com/sarah.jpg",
        "parentCommentId": 1,
        "content": "Thank you so much!",
        "createdAt": "2024-01-15T11:15:00Z",
        "editedAt": null,
        "isEdited": false,
        "likesCount": 1,
        "isLikedByCurrentUser": false,
        "replies": []
      }
    ]
  }
]
```

**Constraints:**
- Returns top-level comments only (parentCommentId = null)
- Replies are nested in the `replies` array
- Ordered by newest first
- Deleted comments are not returned

**Use Case:**
- Display comment section
- Show discussions under posts
- Load more comments pagination

---

### PUT /api/post/comments/{commentId}

**Purpose:** Update/edit a comment (owner only)

**Authentication:** Required

**Path Parameters:**
- `commentId` (required): Integer, comment ID

**Request Body:**
```json
{
  "content": "string"     // Required, updated comment text
}
```

**Constraints:**
- Only comment owner can update
- content: 1-2000 characters
- Cannot change postId or parentCommentId

**Success Response (200 OK):**
```json
{
  "id": 1,
  "postId": 5,
  "userId": 2,
  "username": "bob",
  "displayName": "Bob Builder",
  "profilePictureUrl": "https://example.com/bob.jpg",
  "parentCommentId": null,
  "content": "Updated: This is absolutely amazing!",
  "createdAt": "2024-01-15T11:00:00Z",
  "editedAt": "2024-01-15T11:30:00Z",
  "isEdited": true,
  "likesCount": 3,
  "isLikedByCurrentUser": true,
  "replies": []
}
```

**Error Responses:**
- **403 Forbidden:** Not the comment owner
- **404 Not Found:** Comment doesn't exist

---

### DELETE /api/post/comments/{commentId}

**Purpose:** Delete a comment (owner only)

**Authentication:** Required

**Path Parameters:**
- `commentId` (required): Integer, comment ID

**Constraints:**
- Only comment owner can delete
- Soft delete (marked as deleted)
- Decrements post's commentsCount

**Success Response (204 No Content):**
No response body

**Error Responses:**
- **403 Forbidden:** Not the comment owner
- **404 Not Found:** Comment doesn't exist

---

## Likes

### POST /api/post/{postId}/like

**Purpose:** Toggle like on a post (like if not liked, unlike if already liked)

**Authentication:** Required

**Path Parameters:**
- `postId` (required): Integer, post ID

**Success Response (200 OK):**
```json
{
  "isLiked": true,       // true if now liked, false if unliked
  "likesCount": 11       // Updated total likes count
}
```

**Constraints:**
- One like per user per post
- Toggling: If liked, it unlikes; if not liked, it likes
- Cannot like own post (allowed but you can add validation)

**Error Responses:**
- **404 Not Found:** Post doesn't exist

**Use Case:**
- Show appreciation
- Engage with content
- Track popularity

---

### POST /api/post/comments/{commentId}/like

**Purpose:** Toggle like on a comment

**Authentication:** Required

**Path Parameters:**
- `commentId` (required): Integer, comment ID

**Success Response (200 OK):**
```json
{
  "isLiked": true,
  "likesCount": 5
}
```

**Constraints:**
- One like per user per comment
- Toggling behavior same as posts

**Error Responses:**
- **404 Not Found:** Comment doesn't exist

---

## Direct Messaging

### POST /api/chat/conversations

**Purpose:** Create a new conversation (direct message or group chat)

**Authentication:** Required

**Request Body:**
```json
{
  "participantIds": [2, 3, 4],  // Required, array of user IDs
  "name": "string",              // Optional, required for groups
  "isGroup": false               // Required, true for group chat
}
```

**Constraints:**
- participantIds: At least 1 other user (current user auto-added)
- For direct messages: Exactly 2 participants, reuses existing conversation if found
- For groups: 2+ participants, name required
- name: Max 100 characters, required for groups

**Success Response (200 OK):**
```json
{
  "id": 1,
  "name": "Study Group",
  "isGroup": true,
  "createdAt": "2024-01-15T10:00:00Z",
  "lastMessage": null,
  "participants": [
    {
      "id": 1,
      "username": "sarah123",
      "displayName": "Sarah Smith",
      "profilePictureUrl": "https://example.com/sarah.jpg",
      "isOnline": true,
      "lastSeen": null
    },
    {
      "id": 2,
      "username": "bob",
      "displayName": "Bob Builder",
      "profilePictureUrl": "https://example.com/bob.jpg",
      "isOnline": false,
      "lastSeen": "2024-01-15T09:30:00Z"
    }
  ],
  "unreadCount": 0
}
```

**Use Case:**
- Start private chat with friend
- Create group chat
- Send direct message

**Examples:**

**Direct Message:**
```json
{
  "participantIds": [5],
  "isGroup": false
}
```

**Group Chat:**
```json
{
  "participantIds": [2, 3, 4, 5],
  "name": "Homework Help",
  "isGroup": true
}
```

---

### GET /api/chat/conversations

**Purpose:** Get all conversations for current user

**Authentication:** Required

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Study Group",
    "isGroup": true,
    "createdAt": "2024-01-15T10:00:00Z",
    "lastMessage": {
      "id": 5,
      "conversationId": 1,
      "senderId": 2,
      "senderUsername": "bob",
      "senderDisplayName": "Bob Builder",
      "content": "See you tomorrow!",
      "type": 0,
      "sentAt": "2024-01-15T14:30:00Z",
      "isEdited": false,
      "isDeleted": false
    },
    "participants": [
      {
        "id": 1,
        "username": "sarah123",
        "displayName": "Sarah Smith",
        "profilePictureUrl": "https://example.com/sarah.jpg",
        "isOnline": true,
        "lastSeen": null
      }
    ],
    "unreadCount": 3
  }
]
```

**Constraints:**
- Ordered by most recent message first
- Shows only conversations user is part of
- unreadCount shows messages since last read

**Use Case:**
- Messages inbox
- List all chats
- Show unread counts

---

### GET /api/chat/conversations/{id}

**Purpose:** Get details of a specific conversation

**Authentication:** Required

**Path Parameters:**
- `id` (required): Integer, conversation ID

**Success Response (200 OK):**
```json
{
  "id": 1,
  "name": "Study Group",
  "isGroup": true,
  "createdAt": "2024-01-15T10:00:00Z",
  "lastMessage": {
    "id": 5,
    "conversationId": 1,
    "senderId": 2,
    "senderUsername": "bob",
    "senderDisplayName": "Bob Builder",
    "content": "See you tomorrow!",
    "type": 0,
    "sentAt": "2024-01-15T14:30:00Z",
    "isEdited": false,
    "isDeleted": false
  },
  "participants": [
    {
      "id": 1,
      "username": "sarah123",
      "displayName": "Sarah Smith",
      "profilePictureUrl": "https://example.com/sarah.jpg",
      "isOnline": true,
      "lastSeen": null
    }
  ],
  "unreadCount": 3
}
```

**Error Responses:**
- **403 Forbidden:** Not a participant
- **404 Not Found:** Conversation doesn't exist

---

### GET /api/chat/conversations/{id}/messages

**Purpose:** Get message history for a conversation

**Authentication:** Required

**Path Parameters:**
- `id` (required): Integer, conversation ID

**Query Parameters:**
- `skip` (optional): Number of messages to skip, default 0
- `take` (optional): Number of messages to return, default 50

**Example Request:**
```
GET /api/chat/conversations/1/messages?skip=0&take=50
```

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "conversationId": 1,
    "senderId": 1,
    "senderUsername": "sarah123",
    "senderDisplayName": "Sarah Smith",
    "content": "Hey everyone!",
    "type": 0,
    "sentAt": "2024-01-15T10:05:00Z",
    "isEdited": false,
    "isDeleted": false
  },
  {
    "id": 2,
    "conversationId": 1,
    "senderId": 2,
    "senderUsername": "bob",
    "senderDisplayName": "Bob Builder",
    "content": "Hi Sarah!",
    "type": 0,
    "sentAt": "2024-01-15T10:06:00Z",
    "isEdited": false,
    "isDeleted": false
  }
]
```

**Constraints:**
- Must be conversation participant
- Ordered chronologically (oldest to newest)
- Maximum 50 messages per request
- Deleted messages not returned

**Message Types:**
- `0` - Text
- `1` - Image
- `2` - File
- `3` - System

**Use Case:**
- Display chat history
- Load more messages (pagination)
- Message scrollback

---

### POST /api/chat/messages

**Purpose:** Send a message to a conversation (also available via SignalR for real-time)

**Authentication:** Required

**Request Body:**
```json
{
  "conversationId": 1,     // Required
  "content": "string",     // Required, max 4000 chars
  "type": 0                // Required, message type
}
```

**Constraints:**
- Must be conversation participant
- content: 1-4000 characters
- type: 0 (Text), 1 (Image), 2 (File), 3 (System)

**Success Response (200 OK):**
```json
{
  "id": 10,
  "conversationId": 1,
  "senderId": 1,
  "senderUsername": "sarah123",
  "senderDisplayName": "Sarah Smith",
  "content": "Hello everyone!",
  "type": 0,
  "sentAt": "2024-01-15T15:00:00Z",
  "isEdited": false,
  "isDeleted": false
}
```

**Error Responses:**
- **403 Forbidden:** Not a conversation participant
- **400 Bad Request:** Invalid data

**Note:** For real-time messaging, use SignalR ChatHub instead

---

## Notifications

### GET /api/notification

**Purpose:** Get notifications for current user

**Authentication:** Required

**Query Parameters:**
- `skip` (optional): Number to skip, default 0
- `take` (optional): Number to return, default 20

**Example Request:**
```
GET /api/notification?skip=0&take=20
```

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "title": "New Comment",
    "message": "Bob Builder commented on your post: Great work!",
    "type": 0,
    "isRead": false,
    "createdAt": "2024-01-15T14:30:00Z",
    "data": "{\"PostId\":5,\"CommentId\":10}"
  },
  {
    "id": 2,
    "title": "New Like",
    "message": "Someone liked your post!",
    "type": 0,
    "isRead": true,
    "createdAt": "2024-01-15T13:15:00Z",
    "data": "{\"PostId\":5}"
  }
]
```

**Notification Types:**
- `0` - NewMessage
- `1` - MentionedInMessage
- `2` - AddedToGroup
- `3` - FriendRequest
- `4` - System

**Constraints:**
- Ordered by newest first
- Shows both read and unread

**Use Case:**
- Notification center
- Alert user of activity
- Track engagement

---

### GET /api/notification/unread-count

**Purpose:** Get count of unread notifications

**Authentication:** Required

**Success Response (200 OK):**
```json
5
```

**Use Case:**
- Show notification badge
- Display unread count
- Quick status check

---

### PUT /api/notification/{notificationId}/read

**Purpose:** Mark a specific notification as read

**Authentication:** Required

**Path Parameters:**
- `notificationId` (required): Integer, notification ID

**Success Response (204 No Content):**
No response body

**Constraints:**
- Must be notification owner
- Idempotent (marking as read multiple times is safe)

**Use Case:**
- Mark notification read when viewed
- Clear notification

---

### PUT /api/notification/mark-all-read

**Purpose:** Mark all notifications as read for current user

**Authentication:** Required

**Success Response (204 No Content):**
No response body

**Use Case:**
- Clear all notifications button
- Mark all as read action

---

## Error Codes

### HTTP Status Codes

| Code | Meaning | When It Occurs |
|------|---------|----------------|
| 200 | OK | Successful request |
| 201 | Created | Resource created successfully |
| 204 | No Content | Successful request with no response body |
| 400 | Bad Request | Invalid input, validation errors |
| 401 | Unauthorized | Missing or invalid authentication token |
| 403 | Forbidden | Authenticated but not authorized for action |
| 404 | Not Found | Resource doesn't exist |
| 500 | Internal Server Error | Server-side error |

### Common Error Response Format

```json
{
  "message": "Detailed error message here"
}
```

---

## Data Models

### User Model
```typescript
{
  id: number,
  username: string,
  displayName: string | null,
  profilePictureUrl: string | null,
  isOnline: boolean,
  lastSeen: string | null  // ISO 8601 datetime
}
```

### Post Model
```typescript
{
  id: number,
  userId: number,
  username: string,
  displayName: string | null,
  profilePictureUrl: string | null,
  textContent: string | null,
  type: number,  // 0=Text, 1=Image, 2=Video, 3=TextWithImage, 4=TextWithVideo
  mediaUrl: string | null,
  thumbnailUrl: string | null,
  createdAt: string,  // ISO 8601 datetime
  editedAt: string | null,
  isEdited: boolean,
  likesCount: number,
  commentsCount: number,
  isLikedByCurrentUser: boolean
}
```

### Comment Model
```typescript
{
  id: number,
  postId: number,
  userId: number,
  username: string,
  displayName: string | null,
  profilePictureUrl: string | null,
  parentCommentId: number | null,
  content: string,
  createdAt: string,
  editedAt: string | null,
  isEdited: boolean,
  likesCount: number,
  isLikedByCurrentUser: boolean,
  replies: Comment[]  // Nested replies
}
```

### Conversation Model
```typescript
{
  id: number,
  name: string | null,  // null for direct messages
  isGroup: boolean,
  createdAt: string,
  lastMessage: Message | null,
  participants: User[],
  unreadCount: number
}
```

### Message Model
```typescript
{
  id: number,
  conversationId: number,
  senderId: number,
  senderUsername: string,
  senderDisplayName: string | null,
  content: string,
  type: number,  // 0=Text, 1=Image, 2=File, 3=System
  sentAt: string,
  isEdited: boolean,
  isDeleted: boolean
}
```

### Notification Model
```typescript
{
  id: number,
  title: string,
  message: string,
  type: number,  // 0=NewMessage, 1=MentionedInMessage, 2=AddedToGroup, 3=FriendRequest, 4=System
  isRead: boolean,
  createdAt: string,
  data: string | null  // JSON string with additional context
}
```

---

## Rate Limiting

**Recommended Client-Side Limits:**
- Post creation: Max 10 posts per hour
- Comments: Max 30 comments per hour
- Messages: Max 100 messages per hour
- Likes: No limit (toggleable)

**Note:** Server-side rate limiting should be implemented for production use.

---

## Best Practices

### 1. **Token Management**
- Store tokens securely (not in localStorage if possible, use httpOnly cookies)
- Refresh tokens before expiration
- Clear tokens on logout

### 2. **Pagination**
- Always use pagination for lists
- Typical page sizes: 20-50 items
- Implement infinite scroll or "Load More"

### 3. **Error Handling**
- Always check status codes
- Display user-friendly error messages
- Log errors for debugging

### 4. **Real-time Features**
- Use SignalR for live updates (comments, likes, messages)
- Fall back to polling if WebSocket connection fails
- Reconnect on connection loss

### 5. **Media Handling**
- Upload images/videos to separate storage (AWS S3, Azure Blob, etc.)
- Store only URLs in the database
- Validate file types and sizes on upload
- Generate thumbnails for videos

### 6. **Performance**
- Cache user profiles
- Debounce search queries
- Lazy load images
- Optimize for mobile networks

---

## Examples

### Complete User Flow Example

```javascript
// 1. Register
const registerResponse = await fetch('https://localhost:7001/api/auth/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'sarah123',
    email: 'sarah@example.com',
    password: 'SecurePass123!',
    displayName: 'Sarah Smith'
  })
});
const { token } = await registerResponse.json();

// 2. Create a post
const postResponse = await fetch('https://localhost:7001/api/post', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    textContent: 'Hello world!',
    type: 0
  })
});
const post = await postResponse.json();

// 3. Get feed
const feedResponse = await fetch('https://localhost:7001/api/post/feed?skip=0&take=20', {
  headers: { 'Authorization': `Bearer ${token}` }
});
const feed = await feedResponse.json();

// 4. Like the post
await fetch(`https://localhost:7001/api/post/${post.id}/like`, {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` }
});

// 5. Add a comment
await fetch('https://localhost:7001/api/post/comments', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    postId: post.id,
    content: 'Great post!',
    parentCommentId: null
  })
});
```

---

## Support

For questions or issues:
- Check the main README.md
- Review SETUP_AND_TEST.md for testing guide
- Create an issue in the repository

---

**Last Updated:** January 2024  
**API Version:** 1.0


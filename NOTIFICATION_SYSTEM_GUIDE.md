# Notification System Guide

## 🔔 Enhanced Notification System

The notification system now tracks **WHO** performed each action and sends **personalized notifications** to the right users!

---

## ✨ What's New

### **Before:**
```json
{
  "title": "New Like",
  "message": "Someone liked your post!"  // ❌ No info about who
}
```

### **Now:**
```json
{
  "id": 1,
  "actorUserId": 5,
  "actorUsername": "john_doe",
  "actorDisplayName": "John Doe",
  "actorProfilePictureUrl": "https://example.com/john.jpg",
  "title": "Post Liked",
  "message": "John Doe liked your post!",  // ✅ Shows who did it!
  "type": 3,
  "isRead": false,
  "createdAt": "2024-01-15T14:30:00Z",
  "data": "{\"PostId\":1,\"LikerUserId\":5}"
}
```

---

## 📋 Notification Types

| Type | Value | Description | Who Gets Notified |
|------|-------|-------------|-------------------|
| **PostComment** | 1 | Someone commented on your post | Post owner |
| **CommentReply** | 2 | Someone replied to your comment | Comment owner |
| **PostLike** | 3 | Someone liked your post | Post owner |
| **CommentLike** | 4 | Someone liked your comment | Comment owner |
| **NewMessage** | 0 | New direct message | Message recipient |
| **AddedToGroup** | 6 | Added to group chat | New member |
| **System** | 8 | System notification | Specific user |

---

## 🎯 How Notifications Work

### 1. **Post Comment Notification**

**Trigger:** User B comments on User A's post

**Who gets notified:** User A (post owner)

**Notification details:**
- **Actor:** User B (the commenter)
- **Type:** PostComment (1)
- **Message:** "User B commented on your post: {comment content}"
- **Data:** `{"PostId": 1, "CommentId": 10}`

**Example:**
```json
{
  "actorUserId": 2,
  "actorUsername": "bob",
  "actorDisplayName": "Bob Builder",
  "title": "New Comment",
  "message": "Bob Builder commented on your post: \"Great work!\"",
  "type": 1,
  "data": "{\"PostId\":1,\"CommentId\":10}"
}
```

---

### 2. **Comment Reply Notification**

**Trigger:** User C replies to User B's comment

**Who gets notified:** User B (original commenter)

**Notification details:**
- **Actor:** User C (the replier)
- **Type:** CommentReply (2)
- **Message:** "User C replied to your comment: {reply content}"
- **Data:** `{"PostId": 1, "CommentId": 15, "ParentCommentId": 10}`

**Example:**
```json
{
  "actorUserId": 3,
  "actorUsername": "charlie",
  "actorDisplayName": "Charlie",
  "title": "New Reply",
  "message": "Charlie replied to your comment: \"I agree!\"",
  "type": 2,
  "data": "{\"PostId\":1,\"CommentId\":15,\"ParentCommentId\":10}"
}
```

---

### 3. **Post Like Notification**

**Trigger:** User B likes User A's post

**Who gets notified:** User A (post owner)

**Notification details:**
- **Actor:** User B (the liker)
- **Type:** PostLike (3)
- **Message:** "User B liked your post!"
- **Data:** `{"PostId": 1, "LikerUserId": 2}`

**Example:**
```json
{
  "actorUserId": 2,
  "actorUsername": "bob",
  "actorDisplayName": "Bob Builder",
  "title": "Post Liked",
  "message": "Bob Builder liked your post!",
  "type": 3,
  "data": "{\"PostId\":1,\"LikerUserId\":2}"
}
```

---

### 4. **Comment Like Notification**

**Trigger:** User C likes User B's comment

**Who gets notified:** User B (comment owner)

**Notification details:**
- **Actor:** User C (the liker)
- **Type:** CommentLike (4)
- **Message:** "User C liked your comment!"
- **Data:** `{"CommentId": 10, "LikerUserId": 3}`

**Example:**
```json
{
  "actorUserId": 3,
  "actorUsername": "charlie",
  "actorDisplayName": "Charlie",
  "title": "Comment Liked",
  "message": "Charlie liked your comment!",
  "type": 4,
  "data": "{\"CommentId\":10,\"LikerUserId\":3}"
}
```

---

## 🚫 Smart Notification Rules

The system **won't send notifications** when:
- ✅ You comment on your own post (no self-notification)
- ✅ You reply to your own comment (no self-notification)
- ✅ You like your own post (no self-notification)
- ✅ You like your own comment (no self-notification)

---

## 📊 Complete Activity Flow Example

### Scenario: 3 Users Interacting

**Users:**
- **Alice** (userId: 1) - Creates a post
- **Bob** (userId: 2) - Comments on Alice's post
- **Charlie** (userId: 3) - Likes and replies

**Step 1:** Alice creates a post
```
POST /api/post
{
  "textContent": "Check out my new artwork!",
  "type": 1,
  "mediaFile": [image]
}
```
📭 **No notifications** (it's her own post)

---

**Step 2:** Bob comments on Alice's post
```
POST /api/post/comments
{
  "postId": 1,
  "parentCommentId": null,
  "content": "This is amazing Alice!"
}
```

📬 **Alice receives notification:**
```json
{
  "actorUserId": 2,
  "actorUsername": "bob",
  "actorDisplayName": "Bob Builder",
  "title": "New Comment",
  "message": "Bob Builder commented on your post: \"This is amazing Alice!\"",
  "type": 1
}
```

---

**Step 3:** Charlie likes Alice's post
```
POST /api/post/1/like
```

📬 **Alice receives notification:**
```json
{
  "actorUserId": 3,
  "actorUsername": "charlie",
  "actorDisplayName": "Charlie",
  "title": "Post Liked",
  "message": "Charlie liked your post!",
  "type": 3
}
```

---

**Step 4:** Alice replies to Bob's comment
```
POST /api/post/comments
{
  "postId": 1,
  "parentCommentId": 10,  // Bob's comment ID
  "content": "Thank you Bob!"
}
```

📬 **Bob receives notification:**
```json
{
  "actorUserId": 1,
  "actorUsername": "alice",
  "actorDisplayName": "Alice",
  "title": "New Reply",
  "message": "Alice replied to your comment: \"Thank you Bob!\"",
  "type": 2
}
```

---

**Step 5:** Charlie likes Bob's comment
```
POST /api/post/comments/10/like
```

📬 **Bob receives notification:**
```json
{
  "actorUserId": 3,
  "actorUsername": "charlie",
  "actorDisplayName": "Charlie",
  "title": "Comment Liked",
  "message": "Charlie liked your comment!",
  "type": 4
}
```

---

**Step 6:** Charlie replies to Bob's comment
```
POST /api/post/comments
{
  "postId": 1,
  "parentCommentId": 10,  // Bob's comment ID
  "content": "I agree with Bob!"
}
```

📬 **Bob receives notification:**
```json
{
  "actorUserId": 3,
  "actorUsername": "charlie",
  "actorDisplayName": "Charlie",
  "title": "New Reply",
  "message": "Charlie replied to your comment: \"I agree with Bob!\"",
  "type": 2
}
```

---

## 💡 Summary of Alice's Notifications

After all these interactions, when Alice checks her notifications:

```
GET /api/notification
```

**Alice will see:**
```json
[
  {
    "id": 2,
    "actorUserId": 3,
    "actorUsername": "charlie",
    "actorDisplayName": "Charlie",
    "title": "Post Liked",
    "message": "Charlie liked your post!",
    "type": 3,
    "isRead": false,
    "createdAt": "2024-01-15T10:03:00Z"
  },
  {
    "id": 1,
    "actorUserId": 2,
    "actorUsername": "bob",
    "actorDisplayName": "Bob Builder",
    "title": "New Comment",
    "message": "Bob Builder commented on your post: \"This is amazing Alice!\"",
    "type": 1,
    "isRead": false,
    "createdAt": "2024-01-15T10:01:00Z"
  }
]
```

---

## 💡 Summary of Bob's Notifications

```json
[
  {
    "id": 5,
    "actorUserId": 3,
    "actorUsername": "charlie",
    "actorDisplayName": "Charlie",
    "title": "New Reply",
    "message": "Charlie replied to your comment: \"I agree with Bob!\"",
    "type": 2,
    "isRead": false,
    "createdAt": "2024-01-15T10:06:00Z"
  },
  {
    "id": 4,
    "actorUserId": 3,
    "actorUsername": "charlie",
    "actorDisplayName": "Charlie",
    "title": "Comment Liked",
    "message": "Charlie liked your comment!",
    "type": 4,
    "isRead": false,
    "createdAt": "2024-01-15T10:05:00Z"
  },
  {
    "id": 3,
    "actorUserId": 1,
    "actorUsername": "alice",
    "actorDisplayName": "Alice",
    "title": "New Reply",
    "message": "Alice replied to your comment: \"Thank you Bob!\"",
    "type": 2,
    "isRead": false,
    "createdAt": "2024-01-15T10:04:00Z"
  }
]
```

---

## 🔔 Real-time Notifications via SignalR

When connected to the Notification Hub, you'll receive notifications in real-time:

```javascript
const notificationHub = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/notification?access_token=" + token)
    .withAutomaticReconnect()
    .build();

notificationHub.on("ReceiveNotification", (notification) => {
    console.log("New notification:", notification);
    
    // Show personalized notification
    showToast(
        notification.title,
        notification.message,
        notification.actorProfilePictureUrl
    );
    
    // You can navigate based on the data
    const data = JSON.parse(notification.data);
    if (data.PostId) {
        // Navigate to post
        console.log("Go to post:", data.PostId);
    }
});

await notificationHub.start();
```

---

## 🎨 Frontend Display Examples

### Simple Notification List
```jsx
function NotificationList({ notifications }) {
  return (
    <div className="notifications">
      {notifications.map(notification => (
        <div key={notification.id} className={notification.isRead ? 'read' : 'unread'}>
          <img src={notification.actorProfilePictureUrl || '/default-avatar.png'} />
          <div>
            <strong>{notification.title}</strong>
            <p>{notification.message}</p>
            <small>{formatTime(notification.createdAt)}</small>
          </div>
        </div>
      ))}
    </div>
  );
}
```

### Interactive Notification Item
```jsx
function NotificationItem({ notification }) {
  const data = JSON.parse(notification.data || '{}');
  
  const handleClick = () => {
    // Navigate based on notification type
    if (data.PostId) {
      navigateToPost(data.PostId);
    }
    
    // Mark as read
    markAsRead(notification.id);
  };
  
  return (
    <div onClick={handleClick} className="notification-item">
      <img src={notification.actorProfilePictureUrl || '/default-avatar.png'} />
      <div>
        <p>
          <strong>{notification.actorDisplayName || notification.actorUsername}</strong>
          {' '}
          {getActionText(notification.type)}
        </p>
        {notification.type === 1 || notification.type === 2 ? (
          <p className="preview">{extractCommentFromMessage(notification.message)}</p>
        ) : null}
        <small>{formatTimeAgo(notification.createdAt)}</small>
      </div>
      {!notification.isRead && <div className="unread-badge"></div>}
    </div>
  );
}

function getActionText(type) {
  switch(type) {
    case 1: return 'commented on your post';
    case 2: return 'replied to your comment';
    case 3: return 'liked your post';
    case 4: return 'liked your comment';
    default: return 'interacted with your content';
  }
}
```

---

## 📊 Notification Database Structure

### Notifications Table

| Column | Type | Description |
|--------|------|-------------|
| `Id` | int | Notification ID |
| `UserId` | int | **Who receives** the notification |
| `ActorUserId` | int | **Who performed** the action |
| `Title` | string | Notification title |
| `Message` | string | Personalized message |
| `Type` | int | Notification type (0-8) |
| `IsRead` | bool | Read status |
| `CreatedAt` | datetime | When notification was created |
| `Data` | string | JSON with additional context |

---

## 🧪 Testing Notifications

### Test with 2 Users

**User 1 (Alice):**
```bash
# Register Alice
POST /api/auth/register
{
  "username": "alice",
  "email": "alice@example.com",
  "password": "Test123!",
  "displayName": "Alice Wonder"
}
# Save token as TOKEN_ALICE

# Alice creates a post
POST /api/post
Authorization: Bearer TOKEN_ALICE
{
  "textContent": "My new artwork!",
  "type": 0
}
# Note the post ID (e.g., 1)
```

**User 2 (Bob):**
```bash
# Register Bob
POST /api/auth/register
{
  "username": "bob",
  "email": "bob@example.com",
  "password": "Test123!",
  "displayName": "Bob Builder"
}
# Save token as TOKEN_BOB

# Bob comments on Alice's post
POST /api/post/comments
Authorization: Bearer TOKEN_BOB
{
  "postId": 1,
  "parentCommentId": null,
  "content": "Amazing work Alice!"
}

# Bob likes Alice's post
POST /api/post/1/like
Authorization: Bearer TOKEN_BOB
```

**Check Alice's Notifications:**
```bash
GET /api/notification
Authorization: Bearer TOKEN_ALICE
```

**Alice will see:**
```json
[
  {
    "id": 2,
    "actorUserId": 2,
    "actorUsername": "bob",
    "actorDisplayName": "Bob Builder",
    "actorProfilePictureUrl": null,
    "title": "Post Liked",
    "message": "Bob Builder liked your post!",
    "type": 3,
    "isRead": false,
    "createdAt": "2024-01-15T10:05:00Z",
    "data": "{\"PostId\":1,\"LikerUserId\":2}"
  },
  {
    "id": 1,
    "actorUserId": 2,
    "actorUsername": "bob",
    "actorDisplayName": "Bob Builder",
    "actorProfilePictureUrl": null,
    "title": "New Comment",
    "message": "Bob Builder commented on your post: \"Amazing work Alice!\"",
    "type": 1,
    "isRead": false,
    "createdAt": "2024-01-15T10:02:00Z",
    "data": "{\"PostId\":1,\"CommentId\":10}"
  }
]
```

---

## 🔄 Notification Flow

```
Action                           Notification Recipient       Actor
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Bob comments on Alice's post  →  Alice (post owner)       →  Bob
Charlie replies to Bob        →  Bob (comment owner)      →  Charlie
Charlie likes Alice's post    →  Alice (post owner)       →  Charlie
Bob likes Charlie's comment   →  Charlie (comment owner)  →  Bob
```

---

## 💻 JavaScript Integration

### Fetch and Display Notifications

```javascript
async function fetchNotifications(token) {
  const response = await fetch('https://localhost:7001/api/notification?skip=0&take=20', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  
  const notifications = await response.json();
  
  notifications.forEach(notification => {
    console.log(`${notification.actorDisplayName} ${getAction(notification.type)}`);
    console.log(`Message: ${notification.message}`);
    console.log(`Time: ${notification.createdAt}`);
    console.log('---');
  });
}

function getAction(type) {
  const actions = {
    1: 'commented on your post',
    2: 'replied to your comment',
    3: 'liked your post',
    4: 'liked your comment',
    0: 'sent you a message'
  };
  return actions[type] || 'interacted with you';
}
```

### Real-time Notification Handler
```javascript
notificationHub.on("ReceiveNotification", (notification) => {
  // Parse the data
  const data = JSON.parse(notification.data);
  
  // Show toast notification
  toast.success(notification.message, {
    icon: notification.actorProfilePictureUrl,
    onClick: () => {
      // Navigate to the post/comment
      if (data.PostId) {
        router.push(`/posts/${data.PostId}`);
      }
      // Mark as read
      markNotificationAsRead(notification.id);
    }
  });
  
  // Update unread count badge
  updateUnreadCount(prev => prev + 1);
  
  // Add to notification list
  addNotificationToList(notification);
});
```

---

## 🎯 Benefits

### Before Enhancement:
- ❌ Didn't know who performed the action
- ❌ Generic "Someone" messages
- ❌ No way to display actor's profile picture
- ❌ Limited context in notifications

### After Enhancement:
- ✅ **Actor tracking** - Know exactly who did what
- ✅ **Personalized messages** - "John Doe liked your post"
- ✅ **Actor details** - Username, display name, profile picture
- ✅ **Rich data** - Post IDs, comment IDs for navigation
- ✅ **Smart filtering** - No self-notifications
- ✅ **Specific types** - Different notification types for different actions

---

## 📱 Notification Categories

### **Social Engagement** (Types 1-4)
- Post comments
- Comment replies
- Post likes
- Comment likes

### **Direct Communication** (Type 0)
- New messages
- Group invitations

### **System** (Type 8)
- Welcome messages
- Important updates
- Moderation notices

---

## ✅ Testing Checklist

Test these scenarios:

- [ ] User A creates post, User B comments → **A gets notification**
- [ ] User A creates post, User B likes it → **A gets notification**
- [ ] User A comments, User B replies → **A gets notification**
- [ ] User A comments, User B likes it → **A gets notification**
- [ ] User A creates post, User A comments → **No notification** (self)
- [ ] User A creates post, User A likes it → **No notification** (self)
- [ ] User A gets notification → **Shows B's name and picture**
- [ ] User A clicks notification → **Can navigate to post/comment**
- [ ] User A marks as read → **Notification status updates**
- [ ] Real-time → **Notification appears instantly via SignalR**

---

## 🎨 UI Examples

### Notification Toast
```
┌─────────────────────────────────────┐
│ 🔔 Post Liked                       │
│                                     │
│ [👤] Bob Builder liked your post!  │
│      2 minutes ago                  │
└─────────────────────────────────────┘
```

### Notification List
```
┌─────────────────────────────────────────────┐
│ Notifications (3 unread)                    │
├─────────────────────────────────────────────┤
│ [👤] Bob Builder                       2m   │
│      commented on your post                 │
│      "This is amazing Alice!"               │
│      ● (unread)                             │
├─────────────────────────────────────────────┤
│ [👤] Charlie                           5m   │
│      liked your post                        │
├─────────────────────────────────────────────┤
│ [👤] John Doe                         10m   │
│      replied to your comment                │
│      "Thanks for the tip!"                  │
│      ✓ (read)                               │
└─────────────────────────────────────────────┘
```

---

**Your notification system is now fully personalized and working! 🎉**


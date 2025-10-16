# Friends Management System - Complete Guide

## Overview

A comprehensive friends management system with:
- Friend requests (send, accept, reject, cancel)
- Follow/Unfollow functionality
- Friend suggestions based on mutual friends
- User search and discovery
- Blocking/Unblocking users
- Real-time notifications via SignalR

## 🗄️ Database Models

### Friendship Model
Manages friend relationships and requests.

```csharp
public enum FriendshipStatus
{
    Pending,    // Friend request sent, waiting for response
    Accepted,   // Friend request accepted, now friends
    Rejected,   // Friend request rejected
    Blocked     // User blocked
}
```

**Fields:**
- `RequesterId` - User who sent the friend request
- `AddresseeId` - User who received the friend request
- `Status` - Current status of the friendship
- `CreatedAt` - When request was sent
- `AcceptedAt` - When request was accepted (null if pending/rejected)

### Follow Model
Manages follow relationships (one-way).

**Fields:**
- `FollowerId` - User who is following
- `FollowingId` - User being followed
- `CreatedAt` - When follow relationship was created

## 📡 API Endpoints

All endpoints (except search and public profiles) require authentication with JWT token.

### Friend Requests

#### Send Friend Request
```http
POST /api/friend/requests
Authorization: Bearer {token}
Content-Type: application/json

{
  "addresseeId": 123
}
```

**Response:**
```json
{
  "id": 1,
  "requester": {
    "id": 456,
    "username": "john_doe",
    "displayName": "John Doe",
    "profilePictureUrl": "...",
    "isOnline": true
  },
  "addressee": {
    "id": 123,
    "username": "jane_smith",
    "displayName": "Jane Smith"
  },
  "status": "Pending",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

#### Accept Friend Request
```http
POST /api/friend/requests/{friendshipId}/accept
Authorization: Bearer {token}
```

#### Reject Friend Request
```http
POST /api/friend/requests/{friendshipId}/reject
Authorization: Bearer {token}
```

#### Cancel Friend Request (Sent by you)
```http
DELETE /api/friend/requests/{friendshipId}
Authorization: Bearer {token}
```

#### Get Pending Friend Requests (Received)
```http
GET /api/friend/requests/pending
Authorization: Bearer {token}
```

#### Get Sent Friend Requests
```http
GET /api/friend/requests/sent
Authorization: Bearer {token}
```

### Friends Management

#### Get Friends List
```http
GET /api/friend?userId={userId}&skip=0&take=50
Authorization: Bearer {token}
```

**Parameters:**
- `userId` (optional) - Get friends of specific user (defaults to current user)
- `skip` - Number of friends to skip (pagination)
- `take` - Number of friends to return

**Response:**
```json
{
  "friends": [
    {
      "id": 123,
      "username": "jane_smith",
      "displayName": "Jane Smith",
      "profilePictureUrl": "...",
      "bio": "Hello world!",
      "isOnline": true,
      "lastSeen": "2024-01-15T10:30:00Z"
    }
  ],
  "totalCount": 150
}
```

#### Remove Friend
```http
DELETE /api/friend/{friendId}
Authorization: Bearer {token}
```

#### Get Mutual Friends
```http
GET /api/friend/mutual/{userId}
Authorization: Bearer {token}
```

#### Check if Friends
```http
GET /api/friend/check/{userId}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "areFriends": true
}
```

### Follow/Unfollow

#### Follow User
```http
POST /api/friend/follow/{userId}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "message": "Successfully followed user",
  "isFollowing": true
}
```

#### Unfollow User
```http
DELETE /api/friend/follow/{userId}
Authorization: Bearer {token}
```

#### Get Followers
```http
GET /api/friend/{userId}/followers?skip=0&take=50
Authorization: Bearer {token}
```

#### Get Following
```http
GET /api/friend/{userId}/following?skip=0&take=50
Authorization: Bearer {token}
```

#### Check if Following
```http
GET /api/friend/following/check/{userId}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "isFollowing": true
}
```

### Relationship Status

#### Get Relationship Status with Another User
```http
GET /api/friend/status/{userId}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "areFriends": true,
  "isFollowing": true,
  "isFollowedBy": false,
  "hasPendingRequest": false,
  "friendshipStatus": "Accepted"
}
```

### User Search & Discovery

#### Search Users
```http
GET /api/friend/search?query=john&skip=0&take=20
```

**Public endpoint** - No authentication required

**Response:**
```json
[
  {
    "id": 123,
    "username": "john_doe",
    "displayName": "John Doe",
    "profilePictureUrl": "...",
    "bio": "Software developer",
    "isOnline": true
  }
]
```

#### Get Friend Suggestions
```http
GET /api/friend/suggestions?take=10
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "user": {
      "id": 789,
      "username": "alice_wonder",
      "displayName": "Alice Wonder"
    },
    "mutualFriendsCount": 5,
    "mutualFriends": [
      {
        "id": 123,
        "username": "bob",
        "displayName": "Bob Smith"
      }
    ]
  }
]
```

### User Profile & Stats

#### Get User Profile
```http
GET /api/friend/profile/{userId}
```

**Public endpoint** - Shows relationship status if authenticated

**Response:**
```json
{
  "user": {
    "id": 123,
    "username": "jane_smith",
    "displayName": "Jane Smith",
    "profilePictureUrl": "...",
    "bio": "Hello world!",
    "isOnline": true
  },
  "stats": {
    "friendsCount": 150,
    "followersCount": 200,
    "followingCount": 180,
    "postsCount": 45
  },
  "relationshipStatus": {
    "areFriends": true,
    "isFollowing": true,
    "isFollowedBy": false
  }
}
```

#### Get User Stats
```http
GET /api/friend/stats/{userId}
```

**Public endpoint**

**Response:**
```json
{
  "friendsCount": 150,
  "followersCount": 200,
  "followingCount": 180,
  "postsCount": 45
}
```

### Block User

#### Block User
```http
POST /api/friend/block/{userId}
Authorization: Bearer {token}
```

Blocking a user will:
- Remove friendship if exists
- Remove all follow relationships
- Prevent future friend requests

#### Unblock User
```http
DELETE /api/friend/block/{userId}
Authorization: Bearer {token}
```

#### Get Blocked Users
```http
GET /api/friend/blocked
Authorization: Bearer {token}
```

## 🔔 Notifications

The system automatically sends real-time notifications for:

### Friend Request Received
```json
{
  "type": "FriendRequest",
  "title": "Friend Request",
  "message": "John Doe sent you a friend request!",
  "actorUserId": 456,
  "actorUsername": "john_doe",
  "actorDisplayName": "John Doe"
}
```

### Friend Request Accepted
```json
{
  "type": "FriendRequestAccepted",
  "title": "Friend Request Accepted",
  "message": "Jane Smith accepted your friend request!",
  "actorUserId": 123
}
```

### New Follower
```json
{
  "type": "NewFollower",
  "title": "New Follower",
  "message": "Bob Johnson started following you!",
  "actorUserId": 789
}
```

## 🎯 Usage Examples

### Complete Friend Request Flow

```javascript
// 1. User searches for friends
const searchResults = await fetch('/api/friend/search?query=john', {
  headers: { 'Authorization': `Bearer ${token}` }
});

// 2. Send friend request
await fetch('/api/friend/requests', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({ addresseeId: 123 })
});

// 3. Recipient receives notification via SignalR
// 4. Recipient accepts request
await fetch('/api/friend/requests/1/accept', {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` }
});

// 5. Requester receives notification
// 6. Both users are now friends!
```

### Follow User Flow

```javascript
// Follow a user
await fetch('/api/friend/follow/123', {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` }
});

// User 123 receives notification

// Check if following
const status = await fetch('/api/friend/following/check/123', {
  headers: { 'Authorization': `Bearer ${token}` }
});
// Returns: { "isFollowing": true }
```

### Get Friend Suggestions

```javascript
const suggestions = await fetch('/api/friend/suggestions?take=5', {
  headers: { 'Authorization': `Bearer ${token}` }
});

// Returns users with mutual friends
suggestions.forEach(suggestion => {
  console.log(`${suggestion.user.displayName} - ${suggestion.mutualFriendsCount} mutual friends`);
});
```

## 🧪 Testing in Swagger

1. **Start the app**: `dotnet run`
2. **Navigate to**: `http://localhost:7001/swagger`
3. **Register/Login** to get JWT token
4. **Click Authorize** button and enter token
5. **Try the endpoints**:
   - Search for users
   - Send friend request
   - Accept/reject requests
   - Follow/unfollow users
   - View user profiles

## 🔄 Database Migration

To apply the database changes:

```bash
# Create migration
dotnet ef migrations add AddFriendshipAndFollowSystem

# Apply to database
dotnet ef database update
```

## 🏗️ Architecture

### Service Layer
- `IFriendService` - Interface with all friend management operations
- `FriendService` - Implementation with business logic

### Controller Layer
- `FriendController` - RESTful API endpoints

### Notification Integration
- `INotificationService` - Extended with friend-related notifications
- Real-time delivery via SignalR `NotificationHub`

### Database Layer
- EF Core with proper relationships
- Unique constraints to prevent duplicate friendships/follows
- Cascade delete for data integrity

## 🎨 Frontend Integration Tips

### 1. Friend Request Button States

```javascript
function getFriendButtonState(relationshipStatus) {
  if (relationshipStatus.areFriends) {
    return { text: 'Friends', action: 'remove' };
  }
  if (relationshipStatus.hasPendingRequest) {
    if (relationshipStatus.friendshipStatus === 'Pending') {
      // Check who sent the request
      return { text: 'Pending', action: 'cancel' };
    }
  }
  return { text: 'Add Friend', action: 'send' };
}
```

### 2. Follow Button Toggle

```javascript
function FollowButton({ userId, isFollowing }) {
  const handleClick = async () => {
    if (isFollowing) {
      await unfollow(userId);
    } else {
      await follow(userId);
    }
  };
  
  return (
    <button onClick={handleClick}>
      {isFollowing ? 'Unfollow' : 'Follow'}
    </button>
  );
}
```

### 3. Real-time Notification Handling

```javascript
// Connect to SignalR NotificationHub
connection.on("ReceiveNotification", (notification) => {
  if (notification.type === 'FriendRequest') {
    showNotification(`${notification.actorDisplayName} sent you a friend request!`);
    updateFriendRequestsCount();
  }
  else if (notification.type === 'FriendRequestAccepted') {
    showNotification(`${notification.actorDisplayName} accepted your friend request!`);
    refreshFriendsList();
  }
  else if (notification.type === 'NewFollower') {
    showNotification(`${notification.actorDisplayName} started following you!`);
    updateFollowersCount();
  }
});
```

## 📊 Performance Considerations

- **Pagination**: All list endpoints support skip/take parameters
- **Indexes**: Database indexes on frequently queried fields (RequesterId, AddresseeId, FollowerId, FollowingId, Status)
- **Caching**: Consider caching friend counts for high-traffic profiles
- **Batch Operations**: Use batch queries when loading multiple user profiles

## 🔒 Security Features

- All modification endpoints require authentication
- Users can only accept/reject requests sent to them
- Users can only cancel requests they sent
- Blocked users cannot send friend requests
- Proper authorization checks on all operations

## 🚀 Next Steps

1. **Run migration** to create database tables
2. **Test the API** using Swagger UI
3. **Integrate with frontend** using the examples above
4. **Monitor notifications** to ensure real-time delivery
5. **Consider adding**:
   - Friend lists (favorites, close friends)
   - Friend request expiration
   - Follow notifications toggle
   - Privacy settings for profile visibility

---

**Happy Coding! 🎉**


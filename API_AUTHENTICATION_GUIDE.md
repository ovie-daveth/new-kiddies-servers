# API Authentication Guide

## Authentication Overview

This API uses JWT (JSON Web Token) authentication. Some endpoints are public (no authentication required), while others require you to be logged in.

## Public Endpoints (No Authentication Required) 🌍

These endpoints can be accessed without logging in:

### Posts - Viewing
- **GET** `/api/post/feed` - View all posts in the feed
- **GET** `/api/post/{postId}` - View a specific post
- **GET** `/api/post/user/{userId}` - View all posts by a specific user
- **GET** `/api/post/{postId}/comments` - View comments on a post

**Note:** When viewing posts anonymously, you won't see which posts you've liked. Login to see your likes and interactions.

## Protected Endpoints (Authentication Required) 🔒

You must create an account and be logged in to access these endpoints:

### Authentication
- **POST** `/api/auth/register` - Create a new account (public)
- **POST** `/api/auth/login` - Login to get JWT token (public)

### Posts - Creating & Managing
- **POST** `/api/post` - Create a new post ✅ Requires login
- **PUT** `/api/post/{postId}` - Update your post ✅ Requires login
- **DELETE** `/api/post/{postId}` - Delete your post ✅ Requires login

### Likes
- **POST** `/api/post/{postId}/like` - Like/unlike a post ✅ Requires login
- **POST** `/api/post/comments/{commentId}/like` - Like/unlike a comment ✅ Requires login

### Comments
- **POST** `/api/post/comments` - Add a comment ✅ Requires login
- **PUT** `/api/post/comments/{commentId}` - Update your comment ✅ Requires login
- **DELETE** `/api/post/comments/{commentId}` - Delete your comment ✅ Requires login

### Chat & Messaging
- **All chat endpoints** require authentication ✅

### Notifications
- **All notification endpoints** require authentication ✅

## How to Use Authentication

### Step 1: Register an Account
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "displayName": "John Doe",
  "bio": "Hello world!",
  "dateOfBirth": "2010-01-01"
}
```

### Step 2: Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "johndoe",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "username": "johndoe",
  "email": "john@example.com",
  "displayName": "John Doe"
}
```

### Step 3: Use the Token
Include the token in the `Authorization` header for all protected endpoints:

```http
POST /api/post/{postId}/like
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Testing in Swagger

1. **Register/Login** using the Auth endpoints
2. Copy the `token` from the response
3. Click the **🔓 Authorize** button at the top of Swagger
4. Enter your token in the format: `Bearer YOUR_TOKEN_HERE`
5. Click **Authorize**
6. Now you can test all protected endpoints!

## Testing with curl

### Public endpoint (no auth needed)
```bash
curl http://localhost:5001/api/post/feed
```

### Protected endpoint (auth required)
```bash
# First, login and get token
TOKEN=$(curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"johndoe","password":"SecurePassword123!"}' \
  | jq -r '.token')

# Then use token to like a post
curl -X POST http://localhost:5001/api/post/1/like \
  -H "Authorization: Bearer $TOKEN"
```

## Error Responses

### 401 Unauthorized
You tried to access a protected endpoint without a valid token:
```json
{
  "message": "Unauthorized"
}
```

**Solution:** Login and include the token in the Authorization header.

### 403 Forbidden
You're logged in but don't have permission (e.g., trying to delete someone else's post):
```json
{
  "message": "Forbidden"
}
```

**Solution:** You can only modify your own posts and comments.

## Endpoint Summary Table

| Endpoint | Method | Public? | Requires Auth? | Purpose |
|----------|--------|---------|----------------|---------|
| `/api/auth/register` | POST | ✅ | ❌ | Create account |
| `/api/auth/login` | POST | ✅ | ❌ | Get JWT token |
| `/api/post/feed` | GET | ✅ | ❌ | View all posts |
| `/api/post/{postId}` | GET | ✅ | ❌ | View single post |
| `/api/post/user/{userId}` | GET | ✅ | ❌ | View user's posts |
| `/api/post/{postId}/comments` | GET | ✅ | ❌ | View comments |
| `/api/post` | POST | ❌ | ✅ | Create post |
| `/api/post/{postId}` | PUT | ❌ | ✅ | Update post |
| `/api/post/{postId}` | DELETE | ❌ | ✅ | Delete post |
| `/api/post/{postId}/like` | POST | ❌ | ✅ | Like/unlike post |
| `/api/post/comments` | POST | ❌ | ✅ | Add comment |
| `/api/post/comments/{commentId}` | PUT | ❌ | ✅ | Update comment |
| `/api/post/comments/{commentId}` | DELETE | ❌ | ✅ | Delete comment |
| `/api/post/comments/{commentId}/like` | POST | ❌ | ✅ | Like/unlike comment |
| `/api/chat/*` | ALL | ❌ | ✅ | All chat features |
| `/api/notification/*` | ALL | ❌ | ✅ | All notifications |

## Token Expiration

JWT tokens expire after a configured time (check your `appsettings.json`). When a token expires:

1. You'll receive a **401 Unauthorized** response
2. Login again to get a new token
3. Use the new token for subsequent requests

## Security Best Practices

### ✅ DO
- Store tokens securely (use httpOnly cookies or secure storage)
- Always use HTTPS in production
- Logout when done (clear token from storage)
- Use strong passwords

### ❌ DON'T
- Share your JWT token
- Store tokens in localStorage (XSS vulnerable)
- Use weak passwords
- Include tokens in URL parameters

## Example User Flow

### Anonymous User (Not Logged In)
```
1. Browse posts → GET /api/post/feed ✅
2. View specific post → GET /api/post/123 ✅
3. Read comments → GET /api/post/123/comments ✅
4. Try to like → POST /api/post/123/like ❌ 401 Unauthorized
   → Need to create account!
```

### Logged In User
```
1. Register → POST /api/auth/register ✅
2. Login → POST /api/auth/login ✅ (get token)
3. Browse posts → GET /api/post/feed ✅
4. Like a post → POST /api/post/123/like ✅ (with token)
5. Add comment → POST /api/post/comments ✅ (with token)
6. Create post → POST /api/post ✅ (with token)
```

## Need Help?

- Check Swagger UI at `/swagger` for interactive testing
- See `API_DOCUMENTATION.md` for detailed endpoint documentation
- See `API_EXAMPLES.md` for more code examples


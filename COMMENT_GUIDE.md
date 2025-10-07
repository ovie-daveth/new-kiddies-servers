# Comment System Guide

## 📝 How Comments Work

The comment system supports **two types of comments**:

1. **Top-level comments** - Direct comments on a post
2. **Replies** - Responses to other comments (nested)

---

## 🎯 Creating Comments

### **Top-Level Comment** (First-time comment on a post)

When commenting directly on a post, set `parentCommentId` to `null`:

```json
{
  "postId": 1,
  "parentCommentId": null,    // ← IMPORTANT: Use null for top-level comments
  "content": "Great post!"
}
```

### **Reply to a Comment** (Nested comment)

When replying to an existing comment, set `parentCommentId` to the comment's ID:

```json
{
  "postId": 1,
  "parentCommentId": 5,    // ← ID of the comment you're replying to
  "content": "I agree with you!"
}
```

---

## ⚠️ Common Mistakes

### ❌ **WRONG** - Setting parentCommentId to a number for first comment
```json
{
  "postId": 1,
  "parentCommentId": 1,    // ← This will cause an error!
  "content": "Na so we see am"
}
```
**Error:** `Parent comment not found` or `An error occurred while saving the entity changes`

### ✅ **CORRECT** - Use null for first comment
```json
{
  "postId": 1,
  "parentCommentId": null,    // ← Correct!
  "content": "Na so we see am"
}
```

---

## 🔄 Comment Flow Example

### Step 1: User creates a post
```json
POST /api/post
{
  "textContent": "What's your favorite food?",
  "type": 0
}
// Returns: { "id": 1, ... }
```

### Step 2: Alice comments on the post (Top-level)
```json
POST /api/post/comments
{
  "postId": 1,
  "parentCommentId": null,    // Top-level comment
  "content": "I love pizza!"
}
// Returns: { "id": 10, "postId": 1, "parentCommentId": null, ... }
```

### Step 3: Bob also comments on the post (Top-level)
```json
POST /api/post/comments
{
  "postId": 1,
  "parentCommentId": null,    // Another top-level comment
  "content": "Jollof rice is the best!"
}
// Returns: { "id": 11, "postId": 1, "parentCommentId": null, ... }
```

### Step 4: Charlie replies to Alice's comment (Nested)
```json
POST /api/post/comments
{
  "postId": 1,
  "parentCommentId": 10,    // Replying to Alice's comment (ID: 10)
  "content": "Pizza is great! What's your favorite topping?"
}
// Returns: { "id": 12, "postId": 1, "parentCommentId": 10, ... }
```

### Step 5: Alice replies to Charlie (Nested)
```json
POST /api/post/comments
{
  "postId": 1,
  "parentCommentId": 12,    // Replying to Charlie's comment (ID: 12)
  "content": "I love pepperoni and mushrooms!"
}
// Returns: { "id": 13, "postId": 1, "parentCommentId": 12, ... }
```

---

## 📊 Comment Structure

When you fetch comments, they come nested:

```json
GET /api/post/1/comments

[
  {
    "id": 10,
    "postId": 1,
    "userId": 2,
    "username": "alice",
    "parentCommentId": null,
    "content": "I love pizza!",
    "createdAt": "2024-01-15T10:00:00Z",
    "likesCount": 5,
    "replies": [
      {
        "id": 12,
        "postId": 1,
        "userId": 3,
        "username": "charlie",
        "parentCommentId": 10,
        "content": "Pizza is great! What's your favorite topping?",
        "createdAt": "2024-01-15T10:05:00Z",
        "likesCount": 2,
        "replies": [
          {
            "id": 13,
            "postId": 1,
            "userId": 2,
            "username": "alice",
            "parentCommentId": 12,
            "content": "I love pepperoni and mushrooms!",
            "createdAt": "2024-01-15T10:10:00Z",
            "likesCount": 1,
            "replies": []
          }
        ]
      }
    ]
  },
  {
    "id": 11,
    "postId": 1,
    "userId": 4,
    "username": "bob",
    "parentCommentId": null,
    "content": "Jollof rice is the best!",
    "createdAt": "2024-01-15T10:02:00Z",
    "likesCount": 8,
    "replies": []
  }
]
```

---

## 🧪 Testing in Swagger

### 1. Create a Post
```
POST /api/post
{
  "textContent": "Test post",
  "type": 0
}
```
Copy the post ID from the response.

### 2. Add First Comment
```
POST /api/post/comments
{
  "postId": 1,         // Use the post ID from step 1
  "parentCommentId": null,   // ← Important!
  "content": "First comment!"
}
```
Copy the comment ID from the response.

### 3. Reply to the Comment
```
POST /api/post/comments
{
  "postId": 1,
  "parentCommentId": 10,  // Use the comment ID from step 2
  "content": "Replying to the first comment!"
}
```

---

## 💻 JavaScript Examples

### Add Top-Level Comment
```javascript
async function addComment(token, postId, content) {
  const response = await fetch('https://localhost:7001/api/post/comments', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      postId: postId,
      parentCommentId: null,  // Top-level comment
      content: content
    })
  });
  
  return await response.json();
}

// Usage
const comment = await addComment(token, 1, "Great post!");
console.log('Comment created:', comment);
```

### Reply to a Comment
```javascript
async function replyToComment(token, postId, parentCommentId, content) {
  const response = await fetch('https://localhost:7001/api/post/comments', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      postId: postId,
      parentCommentId: parentCommentId,  // ID of comment to reply to
      content: content
    })
  });
  
  return await response.json();
}

// Usage
const reply = await replyToComment(token, 1, 10, "I agree!");
console.log('Reply created:', reply);
```

### React Component Example
```jsx
function CommentForm({ postId, parentCommentId = null, onCommentAdded }) {
  const [content, setContent] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await fetch('https://localhost:7001/api/post/comments', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({
          postId: postId,
          parentCommentId: parentCommentId,  // null for top-level, ID for replies
          content: content
        })
      });

      if (response.ok) {
        const newComment = await response.json();
        setContent('');
        onCommentAdded(newComment);
      } else {
        const error = await response.json();
        alert(error.message);
      }
    } catch (error) {
      console.error('Error adding comment:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <textarea
        value={content}
        onChange={(e) => setContent(e.target.value)}
        placeholder={parentCommentId ? "Write a reply..." : "Write a comment..."}
        required
      />
      <button type="submit" disabled={loading}>
        {loading ? 'Posting...' : 'Post Comment'}
      </button>
    </form>
  );
}

// Usage for top-level comment
<CommentForm postId={1} onCommentAdded={handleNewComment} />

// Usage for reply
<CommentForm postId={1} parentCommentId={10} onCommentAdded={handleNewReply} />
```

---

## 🚨 Error Messages

| Error | Cause | Solution |
|-------|-------|----------|
| `Post not found` | Invalid postId or post was deleted | Use a valid post ID |
| `Parent comment not found` | parentCommentId doesn't exist or was deleted | Use `null` for top-level or valid comment ID |
| `Parent comment does not belong to this post` | Trying to reply to a comment from a different post | Make sure parentCommentId is from the same post |
| `An error occurred while saving...` | Database constraint violation | Check that parentCommentId is either `null` or a valid comment ID |

---

## ✅ Quick Rules

1. **First comment on a post?** → Use `parentCommentId: null`
2. **Replying to a comment?** → Use `parentCommentId: <comment_id>`
3. **Parent must exist** → Can't reply to deleted or non-existent comments
4. **Parent must be from same post** → Can't reply to comments from other posts

---

## 🔍 Validation

The API now validates:
- ✅ Post exists and is not deleted
- ✅ If parentCommentId is provided, the parent comment exists
- ✅ If parentCommentId is provided, it belongs to the same post
- ✅ Content is not empty (1-2000 characters)

---

**Happy commenting! 💬**


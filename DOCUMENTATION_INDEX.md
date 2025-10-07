# Documentation Index

Welcome to the Kids Social Media Platform API documentation! This guide will help you find the right documentation for your needs.

---

## 📚 Documentation Files

### 🚀 Getting Started

1. **[README.md](README.md)** - Start here!
   - Project overview
   - Features list
   - Technology stack
   - Installation guide
   - Quick examples

2. **[QUICKSTART.md](QUICKSTART.md)** - Get running in 3 minutes
   - Minimal setup steps
   - Quick testing guide
   - SignalR examples
   - Sample use cases

3. **[SETUP_AND_TEST.md](SETUP_AND_TEST.md)** - Complete setup guide
   - Detailed installation
   - Database configuration
   - Step-by-step testing in Swagger
   - SignalR testing with HTML
   - Multiple user scenarios
   - Troubleshooting

---

### 📖 API Reference

4. **[API_DOCUMENTATION.md](API_DOCUMENTATION.md)** ⭐ COMPREHENSIVE
   - **Every endpoint explained in detail**
   - Request/response examples
   - Field constraints
   - Error codes
   - Data models
   - Best practices
   - **USE THIS** for understanding what each endpoint does

5. **[API_QUICK_REFERENCE.md](API_QUICK_REFERENCE.md)** - Quick lookup
   - All endpoints in table format
   - Common request bodies
   - SignalR events reference
   - Field constraints
   - Status codes
   - **USE THIS** for quick lookup during development

6. **[API_EXAMPLES.md](API_EXAMPLES.md)** - Code examples
   - cURL commands for every endpoint
   - JavaScript/Fetch examples
   - SignalR connection examples
   - Complete testing workflows
   - Error handling patterns
   - **USE THIS** for copy-paste examples

---

## 🎯 What Should I Read?

### I'm Just Getting Started
**Read in this order:**
1. [README.md](README.md) - Understand the project
2. [QUICKSTART.md](QUICKSTART.md) - Get it running
3. [SETUP_AND_TEST.md](SETUP_AND_TEST.md) - Test it properly

### I Need to Understand the API
**Read:**
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md) - Complete reference

### I'm Coding Right Now
**Keep Open:**
- [API_QUICK_REFERENCE.md](API_QUICK_REFERENCE.md) - Quick lookup
- [API_EXAMPLES.md](API_EXAMPLES.md) - Copy-paste examples

### I'm Testing with cURL/Postman
**Use:**
- [API_EXAMPLES.md](API_EXAMPLES.md) - All cURL commands

### I'm Building a Frontend
**Read:**
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md) - Full details
- [API_EXAMPLES.md](API_EXAMPLES.md) - JavaScript examples

---

## 📋 Features Coverage

### Authentication & Users
- ✅ Register new users
- ✅ Login with JWT tokens
- ✅ Search for users
- ✅ View user profiles
- ✅ Online/offline status

**Docs:** All API documentation files

### Social Media Posts
- ✅ Create posts (text, image, video)
- ✅ Get feed with pagination
- ✅ View user profiles/posts
- ✅ Edit and delete posts
- ✅ Like posts
- ✅ Real-time post updates

**Docs:**
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md#posts) - Post endpoints
- [API_EXAMPLES.md](API_EXAMPLES.md#post-examples) - Post examples

### Comments & Discussions
- ✅ Comment on posts
- ✅ Reply to comments (nested)
- ✅ Edit and delete comments
- ✅ Like comments
- ✅ Real-time comment updates

**Docs:**
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md#comments) - Comment endpoints
- [API_EXAMPLES.md](API_EXAMPLES.md#comment-examples) - Comment examples

### Direct Messaging
- ✅ One-on-one chats
- ✅ Group chats
- ✅ Message history
- ✅ Typing indicators
- ✅ Read receipts
- ✅ Real-time messaging

**Docs:**
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md#direct-messaging) - Chat endpoints
- [API_EXAMPLES.md](API_EXAMPLES.md#direct-messaging-examples) - Chat examples

### Notifications
- ✅ Real-time notifications
- ✅ Unread counts
- ✅ Mark as read
- ✅ Multiple notification types

**Docs:**
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md#notifications) - Notification endpoints
- [API_EXAMPLES.md](API_EXAMPLES.md#notification-examples) - Notification examples

### Real-time Features (SignalR)
- ✅ Live comments
- ✅ Live likes
- ✅ Live messaging
- ✅ Live notifications
- ✅ Typing indicators
- ✅ Online status

**Docs:**
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md) - SignalR section
- [API_QUICK_REFERENCE.md](API_QUICK_REFERENCE.md#signalr-hubs) - Hub reference
- [API_EXAMPLES.md](API_EXAMPLES.md#signalr-examples) - Hub examples
- [SETUP_AND_TEST.md](SETUP_AND_TEST.md#testing-real-time-features) - Testing guide

---

## 🔍 Quick Lookup

### Common Questions

**Q: How do I authenticate?**
- A: See [API_DOCUMENTATION.md - Authentication](API_DOCUMENTATION.md#authentication)

**Q: What are the post types?**
- A: `0=Text, 1=Image, 2=Video, 3=TextWithImage, 4=TextWithVideo`
- See [API_QUICK_REFERENCE.md](API_QUICK_REFERENCE.md)

**Q: How do I use pagination?**
- A: Use `skip` and `take` query parameters
- Example: `/api/post/feed?skip=0&take=20`
- See [API_DOCUMENTATION.md - Pagination](API_DOCUMENTATION.md#get-apipostfeed)

**Q: How do I connect to SignalR?**
- A: See [API_EXAMPLES.md - SignalR Examples](API_EXAMPLES.md#signalr-examples)

**Q: What's the token expiration?**
- A: 7 days from login
- See [API_DOCUMENTATION.md - Token Details](API_DOCUMENTATION.md#post-apiauthlogin)

**Q: How do I test in Swagger?**
- A: See [SETUP_AND_TEST.md - Testing with Swagger](SETUP_AND_TEST.md#testing-in-swagger-step-by-step)

**Q: Field length limits?**
- A: See [API_QUICK_REFERENCE.md - Field Constraints](API_QUICK_REFERENCE.md#field-constraints)

---

## 🛠️ Development Workflow

### 1. **Setup** (First Time)
```
1. Read README.md
2. Follow SETUP_AND_TEST.md
3. Test in Swagger
4. Run sample cURL commands from API_EXAMPLES.md
```

### 2. **Development** (Daily)
```
1. Keep API_QUICK_REFERENCE.md open
2. Copy examples from API_EXAMPLES.md
3. Reference API_DOCUMENTATION.md for details
```

### 3. **Integration** (Frontend Development)
```
1. Use API_DOCUMENTATION.md for contracts
2. Copy JavaScript examples from API_EXAMPLES.md
3. Implement SignalR from examples
4. Test with multiple users
```

### 4. **Testing** (QA)
```
1. Use SETUP_AND_TEST.md for scenarios
2. Run cURL scripts from API_EXAMPLES.md
3. Test SignalR real-time features
4. Verify error handling
```

---

## 📊 Endpoint Categories

### Public Endpoints (No Auth Required)
- `POST /api/auth/register`
- `POST /api/auth/login`

### Authenticated Endpoints (Token Required)
**All other endpoints require:**
```
Authorization: Bearer {your_jwt_token}
```

**See:** [API_DOCUMENTATION.md - Authentication Header](API_DOCUMENTATION.md#authentication)

---

## 🔗 External Resources

### SignalR Client Libraries
- **JavaScript/TypeScript:** `npm install @microsoft/signalr`
- **C#:** Built-in with ASP.NET Core
- **Java:** [SignalR Java Client](https://github.com/SignalR/java-client)
- **Swift:** [SignalR Swift Client](https://github.com/moozzyk/SignalR-Client-Swift)

### Testing Tools
- **Swagger UI:** Built-in at `/swagger`
- **Postman:** Import cURL examples
- **cURL:** Command-line testing
- **Browser DevTools:** For SignalR debugging

---

## 📝 File Descriptions

| File | Purpose | When to Use |
|------|---------|-------------|
| **README.md** | Project overview | First read, project intro |
| **QUICKSTART.md** | Fast setup | Quick demo, 3-min setup |
| **SETUP_AND_TEST.md** | Detailed guide | Complete setup, troubleshooting |
| **API_DOCUMENTATION.md** | Full API reference | Understand endpoints deeply |
| **API_QUICK_REFERENCE.md** | Condensed lookup | Quick reference during coding |
| **API_EXAMPLES.md** | Code samples | Copy-paste examples |
| **DOCUMENTATION_INDEX.md** | This file | Navigate docs |

---

## 🎓 Learning Path

### Beginner
1. Read [README.md](README.md)
2. Follow [QUICKSTART.md](QUICKSTART.md)
3. Test in Swagger
4. Try cURL examples from [API_EXAMPLES.md](API_EXAMPLES.md)

### Intermediate
1. Read [API_DOCUMENTATION.md](API_DOCUMENTATION.md)
2. Build simple frontend
3. Implement authentication
4. Connect to SignalR

### Advanced
1. Implement all features
2. Handle errors properly
3. Optimize performance
4. Deploy to production

---

## 🚀 Quick Links

- **Setup:** [SETUP_AND_TEST.md](SETUP_AND_TEST.md)
- **API Reference:** [API_DOCUMENTATION.md](API_DOCUMENTATION.md)
- **Quick Lookup:** [API_QUICK_REFERENCE.md](API_QUICK_REFERENCE.md)
- **Code Examples:** [API_EXAMPLES.md](API_EXAMPLES.md)
- **Troubleshooting:** [SETUP_AND_TEST.md#troubleshooting](SETUP_AND_TEST.md#troubleshooting)

---

## ✨ Tips

1. **Start with Swagger** - It's the easiest way to test
2. **Use the examples** - Copy-paste from API_EXAMPLES.md
3. **Keep Quick Reference open** - Save time during development
4. **Test with 2+ users** - Create multiple accounts to test social features
5. **Use SignalR for real-time** - Much better than polling

---

**Happy Coding! 🎉**

Need help? Check the troubleshooting section in [SETUP_AND_TEST.md](SETUP_AND_TEST.md#troubleshooting)


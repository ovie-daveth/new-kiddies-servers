# File Upload Guide - Posts with Media

## 📤 How to Upload Files

The API now accepts **actual file uploads** for images and videos instead of URLs!

---

## 🎯 Creating Posts with Files

### Endpoint: `POST /api/post`

**Content-Type:** `multipart/form-data`

### Form Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `TextContent` | string | Optional | Post text content (max 5000 chars) |
| `Type` | number | Required | 0=Text, 1=Image, 2=Video, 3=TextWithImage, 4=TextWithVideo |
| `MediaFile` | file | Optional | Image or video file |
| `ThumbnailFile` | file | Optional | Thumbnail for video (recommended for videos) |

---

## 📋 File Requirements

### **Images** (for type 1 or 3)
- **Allowed formats:** `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`, `.avif`, `.svg`, `.bmp`, `.tiff`, `.heic`, `.heif`
- **Max size:** 10MB
- **Use case:** Photos, artwork, screenshots, modern image formats

### **Videos & Audio** (for type 2 or 4)
- **Video formats:** `.mp4`, `.mov`, `.avi`, `.webm`, `.mkv`, `.flv`, `.wmv`, `.m4v`, `.3gp`, `.mpeg`, `.mpg`
- **Audio formats:** `.mp3`, `.wav`, `.ogg`, `.m4a`, `.aac`, `.flac`, `.wma`
- **Max size:** 100MB
- **Thumbnail:** Recommended for videos (provide via `ThumbnailFile`)

### **Thumbnails**
- **Allowed formats:** `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`, `.avif`, `.bmp`
- **Max size:** 10MB
- **Use case:** Video preview images, audio cover art

---

## 🔧 Testing in Swagger

### Step 1: Open Swagger
Go to https://localhost:7001/swagger

### Step 2: Authorize
1. Register/Login to get your token
2. Click **Authorize** button
3. Paste your token

### Step 3: Create Post with Image

1. Find `POST /api/post`
2. Click "Try it out"
3. Fill in the form:

```
TextContent: "Check out this cool picture!"
Type: 1
MediaFile: [Click "Choose File" and select an image]
ThumbnailFile: (leave empty for images)
```

4. Click "Execute"

### Step 4: Create Post with Video

```
TextContent: "My awesome video!"
Type: 2
MediaFile: [Select a .mp4 video file]
ThumbnailFile: [Select a .jpg thumbnail]
```

---

## 💻 JavaScript/TypeScript Example

### Using Fetch API

```javascript
async function createPostWithImage(token, textContent, imageFile) {
  const formData = new FormData();
  formData.append('TextContent', textContent);
  formData.append('Type', '1'); // Image type
  formData.append('MediaFile', imageFile);

  const response = await fetch('https://localhost:7001/api/post', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`
      // Don't set Content-Type - browser will set it automatically with boundary
    },
    body: formData
  });

  return await response.json();
}

// Usage with file input
const fileInput = document.getElementById('imageInput');
const imageFile = fileInput.files[0];

const post = await createPostWithImage(
  token, 
  'Check out my photo!', 
  imageFile
);
```

### Create Video Post

```javascript
async function createPostWithVideo(token, textContent, videoFile, thumbnailFile) {
  const formData = new FormData();
  formData.append('TextContent', textContent);
  formData.append('Type', '2'); // Video type
  formData.append('MediaFile', videoFile);
  
  if (thumbnailFile) {
    formData.append('ThumbnailFile', thumbnailFile);
  }

  const response = await fetch('https://localhost:7001/api/post', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`
    },
    body: formData
  });

  return await response.json();
}
```

### React Example

```jsx
import { useState } from 'react';

function CreatePost({ token }) {
  const [textContent, setTextContent] = useState('');
  const [mediaFile, setMediaFile] = useState(null);
  const [type, setType] = useState(0);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append('TextContent', textContent);
    formData.append('Type', type);
    
    if (mediaFile) {
      formData.append('MediaFile', mediaFile);
    }

    try {
      const response = await fetch('https://localhost:7001/api/post', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: formData
      });

      const post = await response.json();
      console.log('Post created:', post);
      
      // Reset form
      setTextContent('');
      setMediaFile(null);
    } catch (error) {
      console.error('Error creating post:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <textarea
        value={textContent}
        onChange={(e) => setTextContent(e.target.value)}
        placeholder="What's on your mind?"
      />
      
      <select value={type} onChange={(e) => setType(Number(e.target.value))}>
        <option value={0}>Text Only</option>
        <option value={1}>Image</option>
        <option value={2}>Video</option>
      </select>

      <input
        type="file"
        accept="image/*,video/*"
        onChange={(e) => setMediaFile(e.target.files[0])}
      />

      <button type="submit">Post</button>
    </form>
  );
}
```

---

## 🧪 Testing with cURL

### Text Only Post
```bash
curl -X POST https://localhost:7001/api/post \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "TextContent=Hello world!" \
  -F "Type=0"
```

### Image Post
```bash
curl -X POST https://localhost:7001/api/post \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "TextContent=Check out this picture!" \
  -F "Type=1" \
  -F "MediaFile=@/path/to/image.jpg"
```

### Video Post with Thumbnail
```bash
curl -X POST https://localhost:7001/api/post \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "TextContent=My awesome video!" \
  -F "Type=2" \
  -F "MediaFile=@/path/to/video.mp4" \
  -F "ThumbnailFile=@/path/to/thumbnail.jpg"
```

---

## 📂 File Storage

Files are stored in the following structure:

```
uploads/
├── images/          # Image files
│   └── {guid}.jpg
├── videos/          # Video files
│   └── {guid}.mp4
└── thumbnails/      # Video thumbnails
    └── {guid}.jpg
```

Each file gets a unique GUID filename to prevent conflicts.

---

## 🌐 Accessing Uploaded Files

Once uploaded, files are accessible at:

```
https://localhost:7001/uploads/{folder}/{filename}
```

**Example URLs:**
- Image: `https://localhost:7001/uploads/images/abc123.jpg`
- Video: `https://localhost:7001/uploads/videos/def456.mp4`
- Thumbnail: `https://localhost:7001/uploads/thumbnails/ghi789.jpg`

These URLs are returned in the post response:
```json
{
  "id": 1,
  "mediaUrl": "/uploads/images/abc123.jpg",
  "thumbnailUrl": null,
  ...
}
```

---

## ⚠️ Error Handling

### Common Errors

**1. Invalid file type:**
```json
{
  "message": "Invalid image file. Allowed: jpg, jpeg, png, gif, webp, avif, svg, bmp, tiff, heic"
}
```

**2. File too large:**
```json
{
  "message": "Image file too large. Max size: 10MB"
}
```

**3. Invalid video/audio file:**
```json
{
  "message": "Invalid video/audio file. Allowed: mp4, mov, avi, webm, mkv, flv, mp3, wav, ogg, and more"
}
```

### Validation Example

```javascript
async function validateAndUpload(file, type) {
  const maxImageSize = 10 * 1024 * 1024; // 10MB
  const maxVideoSize = 100 * 1024 * 1024; // 100MB

  if (type === 'image' && file.size > maxImageSize) {
    throw new Error('Image must be less than 10MB');
  }

  if (type === 'video' && file.size > maxVideoSize) {
    throw new Error('Video/audio must be less than 100MB');
  }

  // Validate file extension
  const allowedImages = ['.jpg', '.jpeg', '.png', '.gif', '.webp', '.avif', '.svg', '.bmp', '.tiff', '.heic'];
  const allowedVideos = ['.mp4', '.mov', '.avi', '.webm', '.mkv', '.flv', '.wmv', '.m4v', '.3gp', '.mpeg'];
  const allowedAudio = ['.mp3', '.wav', '.ogg', '.m4a', '.aac', '.flac', '.wma'];
  
  const ext = file.name.toLowerCase().substring(file.name.lastIndexOf('.'));
  
  if (type === 'image' && !allowedImages.includes(ext)) {
    throw new Error('Invalid image format');
  }

  if (type === 'video' && ![...allowedVideos, ...allowedAudio].includes(ext)) {
    throw new Error('Invalid video/audio format');
  }
}
```

---

## 🎨 Frontend HTML Example

```html
<!DOCTYPE html>
<html>
<head>
    <title>Create Post</title>
</head>
<body>
    <h1>Create New Post</h1>
    
    <form id="postForm">
        <div>
            <label>Text Content:</label><br>
            <textarea id="textContent" rows="4" cols="50"></textarea>
        </div>

        <div>
            <label>Post Type:</label><br>
            <select id="postType">
                <option value="0">Text Only</option>
                <option value="1">Image</option>
                <option value="2">Video</option>
            </select>
        </div>

        <div>
            <label>Media File:</label><br>
            <input type="file" id="mediaFile" accept="image/*,video/*">
        </div>

        <div id="thumbnailSection" style="display:none;">
            <label>Video Thumbnail:</label><br>
            <input type="file" id="thumbnailFile" accept="image/*">
        </div>

        <button type="submit">Create Post</button>
    </form>

    <div id="result"></div>

    <script>
        const token = 'YOUR_JWT_TOKEN'; // Replace with actual token

        document.getElementById('postType').addEventListener('change', function() {
            const thumbnailSection = document.getElementById('thumbnailSection');
            thumbnailSection.style.display = this.value === '2' ? 'block' : 'none';
        });

        document.getElementById('postForm').addEventListener('submit', async function(e) {
            e.preventDefault();

            const formData = new FormData();
            formData.append('TextContent', document.getElementById('textContent').value);
            formData.append('Type', document.getElementById('postType').value);
            
            const mediaFile = document.getElementById('mediaFile').files[0];
            if (mediaFile) {
                formData.append('MediaFile', mediaFile);
            }

            const thumbnailFile = document.getElementById('thumbnailFile').files[0];
            if (thumbnailFile) {
                formData.append('ThumbnailFile', thumbnailFile);
            }

            try {
                const response = await fetch('https://localhost:7001/api/post', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${token}`
                    },
                    body: formData
                });

                const result = await response.json();
                
                if (response.ok) {
                    document.getElementById('result').innerHTML = 
                        `<p style="color: green;">Post created successfully!</p>
                         <pre>${JSON.stringify(result, null, 2)}</pre>`;
                    document.getElementById('postForm').reset();
                } else {
                    document.getElementById('result').innerHTML = 
                        `<p style="color: red;">Error: ${result.message}</p>`;
                }
            } catch (error) {
                document.getElementById('result').innerHTML = 
                    `<p style="color: red;">Error: ${error.message}</p>`;
            }
        });
    </script>
</body>
</html>
```

---

## 🔒 Security Considerations

1. **File Type Validation** - Only allowed extensions are accepted
2. **File Size Limits** - Prevents large file uploads
3. **Unique Filenames** - GUIDs prevent filename conflicts
4. **Path Traversal Protection** - Files saved to designated folders only
5. **Authentication Required** - All uploads require valid JWT token

---

## 📝 Configuration

Current file size limits in `Services/FileUploadService.cs`:

```csharp
public long MaxImageSize => 10 * 1024 * 1024; // 10MB
public long MaxVideoSize => 100 * 1024 * 1024; // 100MB
```

Current allowed extensions:

```csharp
// Images
private readonly string[] _allowedImageExtensions = { 
    ".jpg", ".jpeg", ".png", ".gif", ".webp", ".avif", ".svg", 
    ".bmp", ".tiff", ".tif", ".heic", ".heif" 
};

// Videos & Audio
private readonly string[] _allowedVideoExtensions = { 
    // Video formats
    ".mp4", ".mov", ".avi", ".webm", ".mkv", ".flv", ".wmv", 
    ".m4v", ".3gp", ".mpeg", ".mpg",
    // Audio formats
    ".mp3", ".wav", ".ogg", ".m4a", ".aac", ".flac", ".wma"
};
```

You can modify these arrays to add or remove formats as needed.

---

## ✅ Quick Checklist

- [ ] Create FormData object
- [ ] Set proper post type (1=Image, 2=Video)
- [ ] Add MediaFile for images/videos
- [ ] Add ThumbnailFile for videos (recommended)
- [ ] Include Authorization header with Bearer token
- [ ] Don't set Content-Type header (let browser do it)
- [ ] Handle validation errors (file type, size)
- [ ] Display uploaded media using returned URLs

---

**Happy uploading! 📸🎥**


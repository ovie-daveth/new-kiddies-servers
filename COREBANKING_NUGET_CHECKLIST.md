# CoreBankingService NuGet Package - Quick Setup Checklist

## ✅ Step-by-Step Setup

### 1️⃣ Configure NuGet Feed

**Update `nuget.config`** (line 7) with your actual feed URL:

```xml
<add key="PremiumTrust" value="https://your-actual-feed-url-here" />
```

**Common URLs:**
- Azure DevOps: `https://pkgs.dev.azure.com/{org}/_packaging/{feed}/nuget/v3/index.json`
- Example: `https://pkgs.dev.azure.com/premiumtrust/_packaging/CoreBanking/nuget/v3/index.json`

### 2️⃣ Install Package

**Option A: Using PowerShell Script** ⭐ RECOMMENDED
```powershell
.\install-corebanking-package.ps1
```

With custom feed:
```powershell
.\install-corebanking-package.ps1 -FeedUrl "https://your-feed-url" -Username "your-email" -Password "your-pat-token"
```

**Option B: Manual Installation**
```bash
# Add NuGet source (if not using nuget.config)
dotnet nuget add source "https://your-feed-url" --name "PremiumTrust" --username "your-email" --password "your-pat" --store-password-in-clear-text

# Install package
dotnet add package CoreBankingService

# Restore and build
dotnet restore
dotnet build
```

### 3️⃣ Verify Installation

Check if package is installed:
```bash
dotnet list package
```

You should see:
```
CoreBankingService    [version]
```

### 4️⃣ Update Program.cs

Add these lines to `Program.cs` (after line 127):

```csharp
// Register CoreBanking Communication Service
builder.Services.AddCommunicationService(builder.Configuration);

// Register Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
```

Full context:
```csharp
// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();

// Register Core Banking Service with HttpClient
builder.Services.AddHttpClient<ICoreBankingService, CoreBankingService>();

// Register CoreBanking Communication Service
builder.Services.AddCommunicationService(builder.Configuration);
builder.Services.AddScoped<IEmailService, EmailService>();
```

### 5️⃣ Create Email Service

See `COREBANKING_NUGET_SETUP.md` Step 5 for the complete `EmailService.cs` implementation.

Or use the quick template:

**Create `Services/IEmailService.cs`:**
```csharp
namespace ChatApp.Backend.Services;

public interface IEmailService
{
    Task SendEmailVerification(string firstName, string link, string email);
    Task SendLoginNotification(string firstName, string email);
    Task EmailResetPassword(string firstName, string temporalPassword, string email);
}
```

**Create `Services/EmailService.cs`:**
```csharp
using CoreBankingService.Services.Communications;
using CoreBankingService.Services.Variables.ADServiceVariables;

namespace ChatApp.Backend.Services;

public class EmailService : IEmailService
{
    private readonly ICommunicationService _communicationService;

    public EmailService(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    public async Task SendEmailVerification(string firstName, string link, string email)
    {
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = $"<h2>Hi {firstName},</h2><p><a href='{link}'>Verify Email</a></p>",
            fromEmail = "noreply@premiumtrustbank.com",
            fromName = "PREMIUMTRUST BANK",
            subject = $"{firstName}, Please Verify Your Email",
            toEmail = email
        });
    }

    // Add other methods...
}
```

### 6️⃣ Use in AuthService

Update `Services/AuthService.cs`:

```csharp
public class AuthService : IAuthService
{
    private readonly ChatDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService; // Add this

    public AuthService(
        ChatDbContext context, 
        IConfiguration configuration,
        IEmailService emailService) // Add this
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService; // Add this
    }

    public async Task<AuthResponseDto> Register(RegisterDto dto)
    {
        // ... existing code ...

        // Send verification email
        var verificationLink = $"https://your-app.com/verify?token={user.Id}";
        await _emailService.SendEmailVerification(
            user.DisplayName ?? user.Username, 
            verificationLink, 
            user.Email);

        return new AuthResponseDto { /* ... */ };
    }
}
```

### 7️⃣ Test

```bash
# Build
dotnet build

# Run
dotnet run

# Test with Swagger
# Navigate to: http://localhost:7001/swagger
```

---

## 🚨 Troubleshooting

### Package Not Found
```bash
# Check NuGet sources
dotnet nuget list source

# Verify feed URL is accessible
# Update nuget.config with correct URL
```

### Authentication Error
```bash
# Add source with credentials
dotnet nuget add source "YOUR_URL" --name "PremiumTrust" --username "your-email@company.com" --password "your-personal-access-token" --store-password-in-clear-text
```

### Build Errors After Install
```bash
# Clear cache and restore
dotnet nuget locals all --clear
dotnet restore
dotnet build
```

---

## 📋 Quick Commands

```bash
# Install package
dotnet add package CoreBankingService

# List installed packages
dotnet list package

# Clear cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

---

## 📞 Need the Feed URL?

Contact your team lead or DevOps for:
- ✅ NuGet feed URL
- ✅ Personal Access Token (PAT)
- ✅ Package version to use

---

## ✅ Checklist Summary

- [ ] Update `nuget.config` with feed URL
- [ ] Install CoreBankingService package
- [ ] Create `IEmailService.cs`
- [ ] Create `EmailService.cs`
- [ ] Update `Program.cs` to register services
- [ ] Update `AuthService.cs` to inject EmailService
- [ ] Test build: `dotnet build`
- [ ] Test run: `dotnet run`
- [ ] Test email sending via Swagger

---

**Ready to go! 🚀**

See `COREBANKING_NUGET_SETUP.md` for detailed documentation.


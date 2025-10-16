# CoreBankingService NuGet Package Setup

## Step 1: Configure Custom NuGet Feed

### Option A: Update nuget.config (Already Done ✅)

Edit `nuget.config` and replace `YOUR_CUSTOM_NUGET_FEED_URL` with your actual feed URL:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <!-- Update this with your actual feed URL -->
    <add key="PremiumTrust" value="https://your-feed-url-here" />
  </packageSources>
</configuration>
```

**Common Feed URL Formats:**
- Azure DevOps: `https://pkgs.dev.azure.com/{organization}/_packaging/{feed}/nuget/v3/index.json`
- GitHub Packages: `https://nuget.pkg.github.com/{owner}/index.json`
- Private Server: `https://your-server.com/nuget/v3/index.json`

### Option B: Add NuGet Source via Command Line

```bash
# Add your custom NuGet source
dotnet nuget add source "https://your-feed-url" --name "PremiumTrust"

# If authentication is required
dotnet nuget add source "https://your-feed-url" --name "PremiumTrust" --username "your-username" --password "your-pat-token" --store-password-in-clear-text
```

## Step 2: Install CoreBankingService Package

### Using .NET CLI (Recommended)

```bash
dotnet add package CoreBankingService
```

Or specific version:
```bash
dotnet add package CoreBankingService --version 1.0.0
```

### Using Visual Studio

1. Right-click on project → **Manage NuGet Packages**
2. Select your custom feed from the **Package source** dropdown
3. Search for **CoreBankingService**
4. Click **Install**

### Using Package Manager Console

```powershell
Install-Package CoreBankingService
```

## Step 3: Configure CoreBanking Parameters

Based on your existing setup, update `appsettings.json`:

```json
{
  "AppSettings": {
    "CoreBankingParameterName": "AppSettings/CoreBanking",
    "OnboardingCorrespondence": "Correspondence/Blayz/Onboarding",
    "FromEmail": "AppSettings/PremiumMobile/FromEmail"
  },
  "CoreBankingService": {
    "BaseUrl": "https://your-corebanking-api.com",
    "ApiKey": "your-api-key",
    "Timeout": 30,
    "EnableLogging": true
  }
}
```

## Step 4: Register Services in Program.cs

Update your `Program.cs`:

```csharp
using CoreBankingService.Services.Communications;

// ... existing code ...

// Register CoreBanking Services
builder.Services.AddCommunicationService(builder.Configuration);

// Or if manual registration is needed:
builder.Services.AddScoped<ICommunicationService, CommunicationService>();

// Register your custom services that use CoreBankingService
builder.Services.AddScoped<IEmailService, EmailService>();
```

## Step 5: Create Email Service Using CoreBankingService

Create `Services/IEmailService.cs`:

```csharp
namespace ChatApp.Backend.Services;

public interface IEmailService
{
    Task SendEmailVerification(string firstName, string link, string email);
    Task SendEmailDeviceVerification(string firstName, string link, string email);
    Task SendLoginNotification(string firstName, string email);
    Task SendDeviceChangeNotification(string firstName, string link, string email);
    Task SendPasswordChangeNotification(string firstName, string link, string email);
    Task SendEmailVerified(string firstName, string email);
    Task EmailResetPassword(string firstName, string temporalPassword, string email);
}
```

Create `Services/EmailService.cs`:

```csharp
using Microsoft.Extensions.Options;
using ChatApp.Backend.Configuration;
using CoreBankingService.Services.Communications;
using CoreBankingService.Services.Variables.ADServiceVariables;

namespace ChatApp.Backend.Services;

public class EmailService : IEmailService
{
    private readonly ICommunicationService _communicationService;
    private readonly CoreBankingSettings _settings;
    private readonly string _fromEmail;

    public EmailService(
        ICommunicationService communicationService, 
        IOptions<CoreBankingSettings> settings)
    {
        _communicationService = communicationService;
        _settings = settings.Value;
        _fromEmail = "noreply@premiumtrustbank.com"; // Or from settings
    }

    public async Task SendEmailVerification(string firstName, string link, string email)
    {
        var html = GenerateVerifyEmail(firstName, link);
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = html,
            fromEmail = _fromEmail,
            fromName = "PREMIUMTRUST BANK",
            subject = $"{firstName}, Please Verify Your Email",
            toEmail = email
        });
    }

    public async Task SendLoginNotification(string firstName, string email)
    {
        var html = GenerateLoginNotification(firstName);
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = html,
            fromEmail = _fromEmail,
            fromName = "PREMIUMTRUST BANK",
            subject = "Login Notification",
            toEmail = email
        });
    }

    public async Task SendEmailDeviceVerification(string firstName, string link, string email)
    {
        var html = GenerateDeviceVerifyEmail(firstName, link);
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = html,
            fromEmail = _fromEmail,
            fromName = "PREMIUMTRUST BANK",
            subject = $"{firstName}, Please complete your device change",
            toEmail = email
        });
    }

    public async Task SendDeviceChangeNotification(string firstName, string link, string email)
    {
        var html = GenerateDeviceChange(firstName, link);
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = html,
            fromEmail = _fromEmail,
            fromName = "PREMIUMTRUST BANK",
            subject = "Device Change Notification",
            toEmail = email
        });
    }

    public async Task SendPasswordChangeNotification(string firstName, string link, string email)
    {
        var html = GeneratePasswordChange(firstName, link);
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = html,
            fromEmail = _fromEmail,
            fromName = "PREMIUMTRUST BANK",
            subject = "Password Change Notification",
            toEmail = email
        });
    }

    public async Task SendEmailVerified(string firstName, string email)
    {
        var html = GenerateEmailVerified(firstName);
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = html,
            fromEmail = _fromEmail,
            fromName = "PREMIUMTRUST BANK",
            subject = $"Welcome to Kids Social Platform, {firstName}",
            toEmail = email
        });
    }

    public async Task EmailResetPassword(string firstName, string temporalPassword, string email)
    {
        var html = GenerateResetPasswordEmail(firstName, temporalPassword);
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = html,
            fromEmail = _fromEmail,
            fromName = "PREMIUMTRUST BANK",
            subject = "Reset Forgotten Password",
            toEmail = email
        });
    }

    // Email template generation methods
    private string GenerateVerifyEmail(string firstName, string link)
    {
        return $@"
            <html>
            <body>
                <h2>Hi {firstName},</h2>
                <p>Please verify your email by clicking the link below:</p>
                <a href='{link}'>Verify Email</a>
            </body>
            </html>";
    }

    private string GenerateLoginNotification(string firstName)
    {
        return $@"
            <html>
            <body>
                <h2>Hi {firstName},</h2>
                <p>We noticed a new login to your account.</p>
                <p>If this wasn't you, please secure your account immediately.</p>
            </body>
            </html>";
    }

    private string GenerateDeviceVerifyEmail(string firstName, string link)
    {
        return $@"
            <html>
            <body>
                <h2>Hi {firstName},</h2>
                <p>Please verify your device change by clicking the link below:</p>
                <a href='{link}'>Verify Device</a>
            </body>
            </html>";
    }

    private string GenerateDeviceChange(string firstName, string link)
    {
        return $@"
            <html>
            <body>
                <h2>Hi {firstName},</h2>
                <p>We noticed a device change on your account.</p>
                <a href='{link}'>Review Changes</a>
            </body>
            </html>";
    }

    private string GeneratePasswordChange(string firstName, string link)
    {
        return $@"
            <html>
            <body>
                <h2>Hi {firstName},</h2>
                <p>Your password was recently changed.</p>
                <p>If this wasn't you, please click the link below:</p>
                <a href='{link}'>Secure Account</a>
            </body>
            </html>";
    }

    private string GenerateEmailVerified(string firstName)
    {
        return $@"
            <html>
            <body>
                <h2>Welcome {firstName}!</h2>
                <p>Your email has been verified successfully.</p>
                <p>You can now enjoy all features of our platform.</p>
            </body>
            </html>";
    }

    private string GenerateResetPasswordEmail(string firstName, string temporalPassword)
    {
        return $@"
            <html>
            <body>
                <h2>Hi {firstName},</h2>
                <p>Your temporary password is: <strong>{temporalPassword}</strong></p>
                <p>Please login and change your password immediately.</p>
            </body>
            </html>";
    }
}
```

## Step 6: Update AuthService to Send Emails

Update your `Services/AuthService.cs` to use the email service:

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
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> Register(RegisterDto dto)
    {
        // ... existing registration code ...

        // Send verification email
        var verificationLink = $"https://your-app.com/verify-email?token={user.Id}";
        await _emailService.SendEmailVerification(
            user.DisplayName ?? user.Username, 
            verificationLink, 
            user.Email);

        return new AuthResponseDto { /* ... */ };
    }
}
```

## Step 7: Register EmailService in Program.cs

Add to your `Program.cs`:

```csharp
// Register Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
```

## Step 8: Testing

### Test Email Sending

Create a test controller:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IEmailService _emailService;
    
    public TestController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    [HttpPost("test-email")]
    public async Task<IActionResult> TestEmail([FromBody] string email)
    {
        await _emailService.SendEmailVerification("Test User", "https://example.com", email);
        return Ok("Email sent!");
    }
}
```

## Troubleshooting

### Issue: Package Not Found

**Solution:**
1. Verify your NuGet feed URL is correct
2. Check if you have access to the feed
3. Add authentication if required:
   ```bash
   dotnet nuget add source "YOUR_FEED_URL" --name "PremiumTrust" --username "USERNAME" --password "PAT_TOKEN" --store-password-in-clear-text
   ```

### Issue: Authentication Required

**Solution:**
For Azure DevOps, create a Personal Access Token (PAT):
1. Go to Azure DevOps → User Settings → Personal Access Tokens
2. Create new token with **Packaging (Read)** permission
3. Use as password in the command above

### Issue: DLL Not Loading

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

## Quick Commands Reference

```bash
# Add NuGet source
dotnet nuget add source "YOUR_URL" --name "PremiumTrust"

# List NuGet sources
dotnet nuget list source

# Install package
dotnet add package CoreBankingService

# Restore packages
dotnet restore

# Clear cache
dotnet nuget locals all --clear

# Build project
dotnet build
```

## Next Steps

1. ✅ Update `nuget.config` with your feed URL
2. ✅ Install CoreBankingService package
3. ✅ Register services in Program.cs
4. ✅ Update AuthService to send emails
5. ✅ Test email sending

---

**Need Help?** Check with your team for the correct NuGet feed URL and credentials.


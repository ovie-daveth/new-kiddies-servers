# Install CoreBankingService NuGet Package

## 📋 Step-by-Step Installation

### **Step 1: Get Your NuGet Feed URL**

Ask your DevOps team for:
- ✅ NuGet feed URL
- ✅ Personal Access Token (PAT) if authentication required

**Common formats:**
- Azure DevOps: `https://pkgs.dev.azure.com/{org}/_packaging/{feed}/nuget/v3/index.json`
- GitHub: `https://nuget.pkg.github.com/{owner}/index.json`
- Private server: `https://nuget.yourcompany.com/v3/index.json`

---

### **Step 2: Update nuget.config**

Edit `nuget.config` (line 7) and replace `YOUR_NUGET_FEED_URL_HERE` with your actual URL:

```xml
<add key="PremiumTrust" value="https://pkgs.dev.azure.com/premiumtrust/_packaging/CoreBanking/nuget/v3/index.json" />
```

**If authentication is required**, uncomment lines 13-16 and add your credentials:

```xml
<packageSourceCredentials>
  <PremiumTrust>
    <add key="Username" value="your-email@company.com" />
    <add key="ClearTextPassword" value="your-personal-access-token" />
  </PremiumTrust>
</packageSourceCredentials>
```

---

### **Step 3: Install the Package**

**Option A: Using Command Line** ⭐ (Recommended)

```bash
dotnet add package CoreBankingService
```

Or specific version:
```bash
dotnet add package CoreBankingService --version 1.0.0
```

**Option B: Using the Script**

```powershell
.\install-corebanking-package.ps1
```

With authentication:
```powershell
.\install-corebanking-package.ps1 -FeedUrl "https://your-feed-url" -Username "your-email" -Password "your-pat"
```

**Option C: Using Visual Studio**

1. Right-click project → **Manage NuGet Packages**
2. Click **Settings** (gear icon)
3. Add package source with your feed URL
4. Search for **CoreBankingService**
5. Click **Install**

---

### **Step 4: Verify Installation**

```bash
dotnet list package
```

You should see:
```
CoreBankingService    [version]
```

---

### **Step 5: Update Program.cs**

The CoreBankingService package needs to be registered. Add this line after line 134:

```csharp
// Register CoreBanking Communication Service
builder.Services.AddCommunicationService(builder.Configuration);
```

Full context in `Program.cs`:
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

// Register CoreBanking Communication Service (ADD THIS LINE)
builder.Services.AddCommunicationService(builder.Configuration);
```

---

### **Step 6: Enable CoreBanking in AuthService**

Update `Services/AuthService.cs`:

**Uncomment the using statements (lines 10-12):**
```csharp
using CoreBankingService.Services.Variables.ADServiceVariables;
using CoreBankingService.Services.Communications;
```

**Uncomment the field (line 21):**
```csharp
private readonly ICommunicationService _communicationService;
```

**Update the constructor (lines 22-40):**
```csharp
public AuthService(
    ChatDbContext context, 
    IConfiguration configuration, 
    AppSettings appSettings,
    ICommunicationService communicationService,  // ← Add this parameter
    ILogger<AuthService> logger)
{
    _context = context;
    _configuration = configuration;
    _appSettings = appSettings;
    _communicationService = communicationService;  // ← Add this assignment
    _logger = logger;

    // Test Parameter Store connection on startup
    InitializeParameterStoreAsync().GetAwaiter().GetResult();
}
```

---

### **Step 7: Build and Test**

```bash
# Build
dotnet build

# If successful, run
dotnet run
```

If successful, you'll see:
```
✅ CoreBankingService package loaded
✅ ICommunicationService available
✅ Parameter Store connected (when on VPN)
```

---

## 🧪 Use Communication Services

After installation, you can send emails and SMS:

### Send Email Example:

```csharp
public async Task SendWelcomeEmail(User user)
{
    var html = $@"
        <html>
        <body>
            <h2>Welcome {user.DisplayName}!</h2>
            <p>Thank you for joining our platform.</p>
        </body>
        </html>";

    await _communicationService.SendEmail(new SendEmailRequest
    {
        body = html,
        fromEmail = _fromEmail ?? "noreply@premiumtrustbank.com",
        fromName = "Kids Social Platform",
        subject = $"Welcome {user.DisplayName}!",
        toEmail = user.Email
    });

    _logger.LogInformation("✅ Welcome email sent to {Email}", user.Email);
}
```

### Send SMS Example:

```csharp
public async Task SendVerificationSMS(string phoneNumber, string code)
{
    await _communicationService.SendSMS(new SendSMSRequest
    {
        phoneNumber = phoneNumber,
        message = $"Your verification code is: {code}",
        sender = "KidsSocial"
    });

    _logger.LogInformation("✅ SMS sent to {PhoneNumber}", phoneNumber);
}
```

---

## 🐛 Troubleshooting

### Issue: "Package 'CoreBankingService' not found"

**Solution 1: Check NuGet sources**
```bash
dotnet nuget list source
```

Should show:
```
Registered Sources:
  1.  nuget.org [Enabled]
      https://api.nuget.org/v3/index.json
  2.  PremiumTrust [Enabled]
      https://your-feed-url
```

**Solution 2: Add source manually**
```bash
dotnet nuget add source "https://your-feed-url" --name "PremiumTrust"
```

**With authentication:**
```bash
dotnet nuget add source "https://your-feed-url" --name "PremiumTrust" --username "your-email@company.com" --password "your-personal-access-token" --store-password-in-clear-text
```

### Issue: "Unauthorized" or "401 Unauthorized"

**Solution:** Add credentials to `nuget.config`:

```xml
<packageSourceCredentials>
  <PremiumTrust>
    <add key="Username" value="your-email@company.com" />
    <add key="ClearTextPassword" value="your-personal-access-token" />
  </PremiumTrust>
</packageSourceCredentials>
```

### Issue: Build errors after installation

**Solution:**
```bash
dotnet clean
dotnet restore
dotnet build
```

### Issue: "The type or namespace 'CoreBankingService' could not be found"

**Solution:**
1. Verify package is installed: `dotnet list package`
2. Clear NuGet cache: `dotnet nuget locals all --clear`
3. Restore: `dotnet restore`
4. Build: `dotnet build`

---

## 📚 What Services You'll Get

After installing CoreBankingService, you'll have access to:

### ICommunicationService
- `SendEmail()` - Send emails
- `SendSMS()` - Send SMS messages
- `SendEmailWithAttachment()` - Send emails with attachments

### Request Models (from package):
- `SendEmailRequest` - Email request structure
- `SendSMSRequest` - SMS request structure

### And possibly more:
- Account services
- Transaction services
- BVN validation
- Other banking services

---

## ✅ Installation Checklist

- [ ] Get NuGet feed URL from DevOps
- [ ] Get Personal Access Token (PAT) if needed
- [ ] Update `nuget.config` line 7 with feed URL
- [ ] Add credentials to `nuget.config` if authentication required
- [ ] Run `dotnet add package CoreBankingService`
- [ ] Verify: `dotnet list package` shows CoreBankingService
- [ ] Update `Program.cs` - add `AddCommunicationService()`
- [ ] Update `AuthService.cs` - uncomment CoreBanking code
- [ ] Build: `dotnet build`
- [ ] Run: `dotnet run`
- [ ] Test on VPN with Parameter Store connected

---

## 🚀 Quick Install Commands

```bash
# 1. Add NuGet source (if authentication needed)
dotnet nuget add source "https://your-feed-url" --name "PremiumTrust" --username "email" --password "pat" --store-password-in-clear-text

# 2. Clear cache (optional but recommended)
dotnet nuget locals all --clear

# 3. Install package
dotnet add package CoreBankingService

# 4. Verify installation
dotnet list package

# 5. Restore packages
dotnet restore

# 6. Build
dotnet build

# 7. Run
dotnet run
```

---

## 📞 Contact for Help

**Need credentials?** Contact:
- DevOps team
- Team lead
- Infrastructure team

They'll provide:
- ✅ NuGet feed URL
- ✅ Personal Access Token (PAT)
- ✅ Package version (if specific version required)

---

## 🎯 After Installation

Once installed and configured:

1. ✅ Send welcome emails on registration
2. ✅ Send verification codes via SMS
3. ✅ Send password reset emails
4. ✅ Send notifications via email/SMS
5. ✅ All communication features work

---

**First step: Update `nuget.config` line 7 with your NuGet feed URL, then run:**

```bash
dotnet add package CoreBankingService
```

---

See also:
- `COREBANKING_NUGET_SETUP.md` - Detailed setup guide
- `COREBANKING_NUGET_CHECKLIST.md` - Quick checklist
- `install-corebanking-package.ps1` - Automated installer script


# Parameter Store - Quick Start

## ✅ Setup Complete!

Your app is now configured to fetch settings from **AWS Parameter Store** instead of storing them in appsettings.json.

---

## 🚀 Quick Start (3 Steps)

### Step 1: Set AWS Credentials

**Development (Local):**
```powershell
# Windows PowerShell
$env:AWS_ACCESS_KEY_ID="your-access-key"
$env:AWS_SECRET_ACCESS_KEY="your-secret-key"
$env:AWS_REGION="us-east-1"
```

```bash
# Linux/Mac
export AWS_ACCESS_KEY_ID="your-access-key"
export AWS_SECRET_ACCESS_KEY="your-secret-key"
export AWS_REGION="us-east-1"
```

**Production (AWS):**
- Attach an IAM role with `ssm:GetParameter` permission
- No credentials needed!

### Step 2: Verify Parameter Exists

```bash
aws ssm get-parameter --name "AppSettings/CoreBanking" --with-decryption
```

### Step 3: Use in Your Code

```csharp
using ChatApp.Backend.Configuration;

public class MyService
{
    private readonly AppSettings _appSettings;
    
    public MyService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    
    public async Task DoSomething()
    {
        // Get settings from Parameter Store
        var settings = await _appSettings.GetCoreBankingSettings();
        
        // Use them
        var baseUrl = settings.CoreEnquiry.BaseUrl;
        var apiKey = settings.CoreEnquiry.ApiKey;
    }
}
```

---

## 📖 What You Have Access To

From your Parameter Store `AppSettings/CoreBanking`:

```csharp
var settings = await _appSettings.GetCoreBankingSettings();

// Core Banking
settings.CoreEnquiry.BaseUrl
settings.CoreEnquiry.ApiKey

// Communications
settings.Communications.BaseUrl
settings.Communications.SendEmail
settings.Communications.SendSMS

// SMS Service
settings.SMSService.BaseUrl
settings.SMSService.ApiKey

// And many more...
```

---

## 🧪 Test It

```bash
dotnet run
```

Visit: `http://localhost:7001/swagger`

Create a test endpoint to verify connection (see full guide).

---

## 📚 Full Documentation

- **Complete Guide**: `PARAMETER_STORE_SETUP.md`
- **NuGet Package**: `COREBANKING_NUGET_SETUP.md`

---

## 🔐 Security

- ✅ Secrets stored in AWS (not in code)
- ✅ Encrypted at rest (SecureString)
- ✅ IAM-based access control
- ✅ Automatic credential rotation support

---

## 🎯 Your Current Setup

**Parameter Store Keys:**
- `AppSettings/CoreBanking` - All CoreBanking settings
- `Correspondence/Blayz/Onboarding` - Onboarding correspondence
- `AppSettings/PremiumMobile/FromEmail` - From email address

**Configuration Classes:**
- `AppSettings` - Fetches from Parameter Store
- `CoreBankingParameterStore` - Strongly-typed settings

**Registered in:**
- `Program.cs` (line 117)

---

**Ready to use! Just set your AWS credentials and start fetching settings securely.** 🔒


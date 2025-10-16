# Custom Parameter Store Setup Guide

## Overview

Your application is configured to fetch settings from your **company's custom Parameter Store API** instead of storing sensitive data in appsettings.json.

---

## ✅ What's Been Set Up

### Files Created:
- ✅ `Configuration/AppSettings.cs` - Service to fetch from your Parameter Store API
- ✅ `Configuration/CoreBankingParameterStore.cs` - Strongly-typed classes for your settings

### Files Updated:
- ✅ `appsettings.json` - Added ParameterStore and AppSettings sections
- ✅ `Program.cs` - Registered AppSettings service with HttpClient

---

## 🔧 Configuration

### Step 1: Update `appsettings.json`

Update lines 2-5 with your company's Parameter Store API details:

```json
{
  "ParameterStore": {
    "BaseUrl": "https://your-parameter-store-api.premiumtrustbank.com",
    "ApiKey": "your-api-key-here"
  }
}
```

**Replace with:**
- `BaseUrl` - Your Parameter Store API base URL
- `ApiKey` - Your API key for authentication

### Step 2: Parameter Names (Already Configured)

Your parameter names are already set:
```json
{
  "AppSettings": {
    "CoreBankingParameterName": "AppSettings/CoreBanking",
    "OnboardingCorrespondence": "Correspondence/Blayz/Onboarding",
    "FromEmail": "AppSettings/PremiumMobile/FromEmail"
  }
}
```

---

## 🚀 How It Works

The `AppSettings` service calls your Parameter Store API like this:

```
GET {BaseUrl}/api/parameters/AppSettings/CoreBanking
Headers:
  X-API-Key: your-api-key
```

**Response expected:**
```json
{
  "CoreEnquiry": {
    "isEnabled": true,
    "baseUrl": "https://dev.premiumtrustbank.com/middleware",
    "ApiKey": "f0b8a08e34f8ad9ed0b7d2b1dd620643..."
  },
  "Communications": {
    "baseUrl": "http://192.168.207.10/PremiumServices",
    "sendSMS": "api/Comms/SendSMS",
    "sendEmail": "api/Comms/SendEmail"
  },
  ...
}
```

---

## 💻 Usage in Your Code

### Example 1: Get CoreBanking Settings

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
        // Fetch from Parameter Store: AppSettings/CoreBanking
        var settings = await _appSettings.GetCoreBankingSettings();
        
        // Access your settings
        var baseUrl = settings.CoreEnquiry.BaseUrl;
        // "https://dev.premiumtrustbank.com/middleware"
        
        var apiKey = settings.CoreEnquiry.ApiKey;
        // "f0b8a08e34f8ad9ed0b7d2b1dd620643..."
        
        var smsUrl = settings.SMSService.BaseUrl;
        var smsApiKey = settings.SMSService.ApiKey;
        
        var communicationsUrl = settings.Communications.BaseUrl;
        var sendEmailEndpoint = settings.Communications.SendEmail;
    }
}
```

### Example 2: Get Email Settings

```csharp
public class EmailService : IEmailService
{
    private readonly AppSettings _appSettings;
    private readonly string _fromEmail;
    
    public EmailService(AppSettings appSettings)
    {
        _appSettings = appSettings;
        // Fetch from Parameter Store at construction
        _fromEmail = _appSettings.GetSettingsAsync(_appSettings.FromEmail).Result;
    }
    
    public async Task SendEmail(string to, string subject, string body)
    {
        var coreBanking = await _appSettings.GetCoreBankingSettings();
        
        // Use Communications settings
        var baseUrl = coreBanking.Communications.BaseUrl;
        var sendEmailEndpoint = coreBanking.Communications.SendEmail;
        
        // Make API call...
    }
}
```

### Example 3: With CoreBankingService NuGet Package

```csharp
using CoreBankingService.Services.Communications;
using CoreBankingService.Services.Variables.ADServiceVariables;
using ChatApp.Backend.Configuration;

public class EmailService : IEmailService
{
    private readonly ICommunicationService _communicationService;
    private readonly AppSettings _appSettings;
    private readonly string _fromEmail;

    public EmailService(
        ICommunicationService communicationService,
        AppSettings appSettings)
    {
        _communicationService = communicationService;
        _appSettings = appSettings;
        _fromEmail = _appSettings.GetSettingsAsync(_appSettings.FromEmail).Result;
    }

    public async Task SendEmailVerification(string firstName, string link, string email)
    {
        var html = $"<h2>Hi {firstName},</h2><p><a href='{link}'>Verify Email</a></p>";
        
        await _communicationService.SendEmail(new SendEmailRequest
        {
            body = html,
            fromEmail = _fromEmail,
            fromName = "PREMIUMTRUST BANK",
            subject = $"{firstName}, Please Verify Your Email",
            toEmail = email
        });
    }
}
```

---

## 🗂️ Your Parameter Store Structure

### Parameter: `AppSettings/CoreBanking`

Your JSON structure contains:
- ✅ **CoreEnquiry** - Core banking enquiry settings
- ✅ **CoreParameters** - Core banking parameters
- ✅ **BvnParameter** - BVN validation settings
- ✅ **AccountOpeningParameter** - Account opening API settings
- ✅ **Authentication** - AD authentication settings
- ✅ **Communications** - SMS/Email communication settings
- ✅ **NipParameter** - NIP transfer settings
- ✅ **Externals** - External services settings
- ✅ **Mandate** - Mandate settings
- ✅ **NEFT** - NEFT transfer settings
- ✅ **VirtualAccountParameter** - Virtual account settings
- ✅ **SMSService** - SMS service settings
- ✅ **AccountSegmentation** - Account segmentation
- ✅ **AccountStatementWithStampSetting** - Statement settings
- ✅ **VirtualCardParameter** - Virtual card settings
- ✅ **VisaCardIssuanceParameter** - Visa card issuance settings

### Access Examples:

```csharp
var settings = await _appSettings.GetCoreBankingSettings();

// Core Banking
var coreUrl = settings.CoreEnquiry.BaseUrl;
var coreApiKey = settings.CoreEnquiry.ApiKey;

// Communications
var commsUrl = settings.Communications.BaseUrl;
var sendSMS = settings.Communications.SendSMS;
var sendEmail = settings.Communications.SendEmail;

// SMS Service
var smsUrl = settings.SMSService.BaseUrl;
var smsApiKey = settings.SMSService.ApiKey;

// BVN Validation
var bvnUrl = settings.BvnParameter.BaseUrl;
var verifySingleBVn = settings.BvnParameter.VerifySingleBVn;

// Virtual Account
var virtualAccountUrl = settings.VirtualAccountParameter.VirtualAccountCredentials.BaseUrl;
var virtualAccountClientId = settings.VirtualAccountParameter.VirtualAccountCredentials.ClientId;
```

---

## 🔍 Available Methods

### AppSettings Methods:

| Method | Description | Returns |
|--------|-------------|---------|
| `GetCoreBankingSettings()` | Get all CoreBanking settings | `CoreBankingParameterStore` |
| `GetSettingsAsync<T>(string)` | Get typed settings | `T` |
| `GetSettingsAsync(string)` | Get string value | `string` |
| `ClearCache()` | Clear cached parameters | `void` |

---

## 🧪 Testing

### Test Parameter Store Connection:

Create a test controller:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TestParameterStoreController : ControllerBase
{
    private readonly AppSettings _appSettings;
    
    public TestParameterStoreController(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    
    [HttpGet("test")]
    public async Task<IActionResult> TestParameterStore()
    {
        try
        {
            var settings = await _appSettings.GetCoreBankingSettings();
            
            return Ok(new
            {
                success = true,
                coreEnquiryUrl = settings.CoreEnquiry.BaseUrl,
                communicationsUrl = settings.Communications.BaseUrl,
                smsUrl = settings.SMSService.BaseUrl,
                message = "Successfully fetched from Parameter Store!"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                error = ex.Message,
                message = "Failed to fetch from Parameter Store"
            });
        }
    }
}
```

**Test it:**
```
GET http://localhost:7001/api/testparameterstore/test
```

---

## 🔐 Security

- ✅ API Key authentication
- ✅ HTTPS connection
- ✅ Settings cached in memory (fast subsequent calls)
- ✅ No secrets in appsettings.json or code

---

## 🚨 Troubleshooting

### Issue: "Connection Refused" or "404 Not Found"

**Solution:**
Check your `ParameterStore:BaseUrl` in appsettings.json. Make sure it's correct.

### Issue: "Unauthorized" or "403 Forbidden"

**Solution:**
Check your `ParameterStore:ApiKey` is correct and has permission to access the parameters.

### Issue: "Parameter Not Found"

**Solution:**
Verify the parameter name in your Parameter Store matches exactly:
- `AppSettings/CoreBanking`
- `Correspondence/Blayz/Onboarding`
- `AppSettings/PremiumMobile/FromEmail`

### Issue: Slow First Request

**Solution:**
This is normal! First request fetches from Parameter Store and caches the result. Subsequent requests are instant.

---

## 📊 Caching

The `AppSettings` service automatically caches fetched parameters in memory. To refresh:

```csharp
_appSettings.ClearCache();
var freshSettings = await _appSettings.GetCoreBankingSettings();
```

---

## 🔄 API Endpoint Format

If your Parameter Store API uses a different endpoint format, update `Configuration/AppSettings.cs` (line 55 & 91):

**Current format:**
```csharp
var url = $"{_parameterStoreBaseUrl}/api/parameters/{parameterName}";
```

**If your API uses a different format, change to:**
```csharp
// Example: /parameters/get?key=AppSettings/CoreBanking
var url = $"{_parameterStoreBaseUrl}/parameters/get?key={parameterName}";

// Example: /v1/config/{parameterName}
var url = $"{_parameterStoreBaseUrl}/v1/config/{parameterName}";
```

---

## ✅ Setup Checklist

- [ ] Update `ParameterStore:BaseUrl` in appsettings.json
- [ ] Update `ParameterStore:ApiKey` in appsettings.json
- [ ] Verify parameters exist in your Parameter Store
- [ ] Test connection with test controller
- [ ] Build and run: `dotnet run`
- [ ] Test at `http://localhost:7001/api/testparameterstore/test`

---

## 📞 Need Help?

Contact your team's infrastructure/DevOps team for:
- ✅ Parameter Store API URL
- ✅ API Key
- ✅ API endpoint format
- ✅ Available parameter names

---

**Your app is now connected to your company's Parameter Store! 🔒**


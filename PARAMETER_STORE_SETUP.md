# AWS Parameter Store Setup Guide

## Overview

Your application is now configured to fetch settings securely from **AWS Systems Manager Parameter Store** instead of storing sensitive data in appsettings.json.

## ✅ What's Been Set Up

### Files Created:
- ✅ `Configuration/AppSettings.cs` - Service to fetch from Parameter Store
- ✅ `Configuration/CoreBankingParameterStore.cs` - Strongly-typed classes for your Parameter Store JSON

### Files Updated:
- ✅ `appsettings.json` - Added AppSettings section with parameter names
- ✅ `Program.cs` - Registered AppSettings service
- ✅ `ChatApp.Backend.csproj` - Added AWSSDK.SimpleSystemsManagement package

### AWS Package Installed:
- ✅ AWSSDK.SimpleSystemsManagement v4.0.4.5

---

## 🔧 AWS Credentials Setup

The AppSettings service uses the **AWS Default Credential Provider Chain** which checks for credentials in this order:

### 1. Environment Variables (Development)
```bash
export AWS_ACCESS_KEY_ID="your-access-key"
export AWS_SECRET_ACCESS_KEY="your-secret-key"
export AWS_REGION="us-east-1"
```

On Windows (PowerShell):
```powershell
$env:AWS_ACCESS_KEY_ID="your-access-key"
$env:AWS_SECRET_ACCESS_KEY="your-secret-key"
$env:AWS_REGION="us-east-1"
```

### 2. AWS Profile (Development)
Create `~/.aws/credentials`:
```ini
[default]
aws_access_key_id = your-access-key
aws_secret_access_key = your-secret-key
```

Create `~/.aws/config`:
```ini
[default]
region = us-east-1
```

### 3. IAM Role (Production - Recommended)
When running on AWS (EC2, ECS, Lambda), the application automatically uses the attached IAM role.

**Required IAM Permissions:**
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ssm:GetParameter",
        "ssm:GetParameters"
      ],
      "Resource": [
        "arn:aws:ssm:us-east-1:*:parameter/AppSettings/*",
        "arn:aws:ssm:us-east-1:*:parameter/Correspondence/*"
      ]
    },
    {
      "Effect": "Allow",
      "Action": [
        "kms:Decrypt"
      ],
      "Resource": "arn:aws:kms:us-east-1:*:key/*"
    }
  ]
}
```

---

## 📖 How to Use

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
        // Get CoreBanking settings from Parameter Store
        var coreBankingSettings = await _appSettings.GetCoreBankingSettings();
        
        // Access specific settings
        var baseUrl = coreBankingSettings.CoreEnquiry.BaseUrl;
        var apiKey = coreBankingSettings.CoreEnquiry.ApiKey;
        var smsBaseUrl = coreBankingSettings.SMSService.BaseUrl;
        
        Console.WriteLine($"CoreBanking URL: {baseUrl}");
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
        var communicationsBaseUrl = coreBanking.Communications.BaseUrl;
        
        // Use ICommunicationService from CoreBankingService NuGet package
        // Send email using the settings
    }
}
```

### Example 3: Use with CoreBankingService NuGet Package

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
        // Fetch from Parameter Store
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

Based on your provided JSON, you have this structure:

### Parameter: `AppSettings/CoreBanking`

Contains:
- CoreEnquiry
- CoreParameters
- BvnParameter
- AccountOpeningParameter
- Authentication
- Communications
- NipParameter
- Externals
- Mandate
- NEFT
- VirtualAccountParameter
- SMSService
- AccountSegmentation
- AccountStatementWithStampSetting
- VirtualCardParameter
- VisaCardIssuanceParameter

### Access Example:

```csharp
var settings = await _appSettings.GetCoreBankingSettings();

// Communications
var communicationsBaseUrl = settings.Communications.BaseUrl;
var sendEmailEndpoint = settings.Communications.SendEmail;

// SMS Service
var smsApiKey = settings.SMSService.ApiKey;

// Core Enquiry
var coreBaseUrl = settings.CoreEnquiry.BaseUrl;
var coreApiKey = settings.CoreEnquiry.ApiKey;
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
    
    [HttpGet("test-parameter-store")]
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
                message = "Successfully fetched from Parameter Store!"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

---

## 🔐 Security Best Practices

### 1. Use Secure String Parameters

When creating parameters in AWS:
```bash
aws ssm put-parameter \
  --name "/AppSettings/CoreBanking" \
  --value '{"CoreEnquiry": {...}}' \
  --type "SecureString" \
  --key-id "alias/aws/ssm"
```

### 2. Restrict IAM Permissions

Only grant access to specific parameter paths your app needs.

### 3. Enable Parameter Versioning

Track changes to your parameters:
```bash
aws ssm get-parameter-history --name "/AppSettings/CoreBanking"
```

### 4. Use Different Parameters Per Environment

```
/AppSettings/Dev/CoreBanking
/AppSettings/Staging/CoreBanking
/AppSettings/Prod/CoreBanking
```

Update `appsettings.Development.json`:
```json
{
  "AppSettings": {
    "CoreBankingParameterName": "AppSettings/Dev/CoreBanking"
  }
}
```

---

## 🚨 Troubleshooting

### Issue: "Access Denied" Error

**Solution:**
Check IAM permissions. User/Role must have `ssm:GetParameter` permission.

### Issue: "Parameter Not Found"

**Solution:**
Verify the parameter name matches exactly:
```bash
aws ssm get-parameter --name "AppSettings/CoreBanking" --with-decryption
```

### Issue: "Region Not Found"

**Solution:**
Set AWS region in `Configuration/AppSettings.cs` (line 34):
```csharp
_ssmClient = new AmazonSimpleSystemsManagementClient(RegionEndpoint.USEast1);
```

Or via environment variable:
```bash
export AWS_REGION="us-east-1"
```

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

## 🔄 Next Steps

1. ✅ Set up AWS credentials (environment variables or IAM role)
2. ✅ Verify your Parameter Store parameters exist
3. ✅ Test connection with the test controller above
4. ✅ Install CoreBankingService NuGet package (when URL is ready)
5. ✅ Create services that use the settings
6. ✅ Deploy to AWS with IAM role

---

## 📞 AWS CLI Commands Reference

```bash
# Get parameter
aws ssm get-parameter --name "AppSettings/CoreBanking" --with-decryption

# List all parameters
aws ssm describe-parameters

# Update parameter
aws ssm put-parameter \
  --name "AppSettings/CoreBanking" \
  --value "$(cat parameter.json)" \
  --type "SecureString" \
  --overwrite

# Delete parameter
aws ssm delete-parameter --name "AppSettings/CoreBanking"
```

---

**Your app is now securely connected to AWS Parameter Store! 🔒**


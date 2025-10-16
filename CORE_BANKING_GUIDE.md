# Core Banking Integration Guide

## Overview

This guide explains how to integrate and use the Core Banking service in your application.

## 📁 Files Created

1. **`Configuration/CoreBankingSettings.cs`** - Strongly-typed configuration class
2. **`Services/ICoreBankingService.cs`** - Service interface
3. **`Services/CoreBankingService.cs`** - Service implementation
4. **`appsettings.json`** - Core Banking configuration
5. **`appsettings.Development.json`** - Development-specific settings

## ⚙️ Configuration

### appsettings.json

```json
{
  "CoreBanking": {
    "BaseUrl": "https://api.corebanking.com",
    "ApiKey": "your-api-key-here",
    "ApiSecret": "your-api-secret-here",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "MerchantId": "your-merchant-id",
    "TimeoutSeconds": 30,
    "EnableRetry": true,
    "MaxRetryAttempts": 3,
    "EnableLogging": true,
    "Environment": "Production",
    "AccountCreationEndpoint": "/api/accounts/create",
    "BalanceInquiryEndpoint": "/api/accounts/balance",
    "TransactionEndpoint": "/api/transactions",
    "TransferEndpoint": "/api/transfers",
    "WebhookCallbackUrl": "https://your-app.com/api/webhooks/corebanking",
    "WebhookSecret": "your-webhook-secret",
    "UseSandbox": false,
    "SandboxBaseUrl": "https://sandbox.corebanking.com",
    "DefaultCurrency": "NGN",
    "BankCode": "000"
  }
}
```

### Configuration Properties

| Property | Type | Description | Required |
|----------|------|-------------|----------|
| `BaseUrl` | string | Core Banking API base URL | ✅ Yes |
| `ApiKey` | string | API key for authentication | ✅ Yes |
| `ApiSecret` | string | API secret for authentication | ✅ Yes |
| `ClientId` | string | Client ID for OAuth | ✅ Yes |
| `ClientSecret` | string | Client secret for OAuth | ✅ Yes |
| `MerchantId` | string | Your merchant/institution ID | No |
| `TimeoutSeconds` | int | API request timeout (default: 30) | No |
| `EnableRetry` | bool | Enable automatic retry on failure | No |
| `MaxRetryAttempts` | int | Maximum retry attempts (default: 3) | No |
| `EnableLogging` | bool | Enable request/response logging | No |
| `Environment` | string | Environment name (Development/Production) | No |
| `UseSandbox` | bool | Use sandbox environment for testing | No |
| `SandboxBaseUrl` | string | Sandbox API base URL | No |
| `DefaultCurrency` | string | Default currency code (e.g., NGN, USD) | No |
| `BankCode` | string | Your bank code or routing number | No |

## 🔧 Setup

### 1. Configuration is Already Registered

The CoreBankingSettings is already registered in `Program.cs`:

```csharp
// Configure strongly-typed settings
builder.Services.Configure<CoreBankingSettings>(
    builder.Configuration.GetSection("CoreBanking"));

// Register Core Banking Service with HttpClient
builder.Services.AddHttpClient<ICoreBankingService, CoreBankingService>();
```

### 2. Update Your Settings

Update the values in `appsettings.json` with your actual Core Banking credentials:

```json
{
  "CoreBanking": {
    "BaseUrl": "https://your-actual-banking-api.com",
    "ApiKey": "your-real-api-key",
    "ClientId": "your-real-client-id",
    // ... other settings
  }
}
```

### 3. For Production

Create `appsettings.Production.json`:

```json
{
  "CoreBanking": {
    "UseSandbox": false,
    "EnableLogging": false,
    "Environment": "Production",
    "BaseUrl": "https://production.corebanking.com",
    "ApiKey": "prod-api-key",
    "ClientId": "prod-client-id"
  }
}
```

## 💻 Usage Examples

### Example 1: Inject in a Controller

```csharp
using ChatApp.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankingController : ControllerBase
{
    private readonly ICoreBankingService _coreBankingService;
    
    public BankingController(ICoreBankingService coreBankingService)
    {
        _coreBankingService = coreBankingService;
    }
    
    [HttpPost("accounts/create")]
    public async Task<ActionResult<BankAccountDto>> CreateAccount(
        [FromBody] CreateAccountRequest request)
    {
        try
        {
            var account = await _coreBankingService.CreateAccount(
                request.UserId, 
                request.AccountType);
                
            return Ok(account);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("accounts/{accountNumber}/balance")]
    public async Task<ActionResult<decimal>> GetBalance(string accountNumber)
    {
        try
        {
            var balance = await _coreBankingService.GetAccountBalance(accountNumber);
            return Ok(new { accountNumber, balance });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPost("transfer")]
    public async Task<ActionResult<TransactionResultDto>> Transfer(
        [FromBody] TransferRequest request)
    {
        try
        {
            var result = await _coreBankingService.Transfer(
                request.FromAccount,
                request.ToAccount,
                request.Amount,
                request.Narration);
                
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CreateAccountRequest
{
    public int UserId { get; set; }
    public string AccountType { get; set; } = string.Empty;
}

public class TransferRequest
{
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
}
```

### Example 2: Inject in a Service

```csharp
using ChatApp.Backend.Services;

namespace ChatApp.Backend.Services;

public class WalletService : IWalletService
{
    private readonly ICoreBankingService _coreBankingService;
    private readonly ChatDbContext _context;
    
    public WalletService(
        ICoreBankingService coreBankingService,
        ChatDbContext context)
    {
        _coreBankingService = coreBankingService;
        _context = context;
    }
    
    public async Task<string> CreateUserWallet(int userId)
    {
        // Create account in core banking
        var bankAccount = await _coreBankingService.CreateAccount(userId, "SAVINGS");
        
        // Save account details to your database
        var wallet = new Wallet
        {
            UserId = userId,
            AccountNumber = bankAccount.AccountNumber,
            AccountName = bankAccount.AccountName,
            Balance = bankAccount.Balance,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        
        return bankAccount.AccountNumber;
    }
    
    public async Task<decimal> GetWalletBalance(int userId)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
            
        if (wallet == null)
            throw new Exception("Wallet not found");
            
        // Get real-time balance from core banking
        var balance = await _coreBankingService.GetAccountBalance(wallet.AccountNumber);
        
        // Update local balance
        wallet.Balance = balance;
        await _context.SaveChangesAsync();
        
        return balance;
    }
}
```

### Example 3: Using Configuration Settings Directly

```csharp
using ChatApp.Backend.Configuration;
using Microsoft.Extensions.Options;

public class MyService
{
    private readonly CoreBankingSettings _settings;
    
    public MyService(IOptions<CoreBankingSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public void DoSomething()
    {
        // Access configuration values
        var apiUrl = _settings.GetEffectiveBaseUrl(); // Returns sandbox or prod URL
        var currency = _settings.DefaultCurrency;
        var isValid = _settings.IsValid(); // Validates required fields
        
        Console.WriteLine($"Using API: {apiUrl}");
        Console.WriteLine($"Currency: {currency}");
        Console.WriteLine($"Sandbox Mode: {_settings.UseSandbox}");
    }
}
```

## 🧪 Testing

### Unit Testing with Mock

```csharp
using Moq;
using Xunit;

public class BankingControllerTests
{
    [Fact]
    public async Task CreateAccount_ReturnsOk()
    {
        // Arrange
        var mockService = new Mock<ICoreBankingService>();
        mockService.Setup(s => s.CreateAccount(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new BankAccountDto
            {
                AccountNumber = "1234567890",
                AccountName = "Test User",
                Balance = 0,
                Currency = "NGN"
            });
            
        var controller = new BankingController(mockService.Object);
        
        // Act
        var result = await controller.CreateAccount(new CreateAccountRequest
        {
            UserId = 1,
            AccountType = "SAVINGS"
        });
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var account = Assert.IsType<BankAccountDto>(okResult.Value);
        Assert.Equal("1234567890", account.AccountNumber);
    }
}
```

### Integration Testing with Sandbox

Set `UseSandbox: true` in `appsettings.Development.json` to test with sandbox API:

```csharp
// Your code will automatically use sandbox URLs
var account = await _coreBankingService.CreateAccount(userId, "SAVINGS");
// This calls: https://sandbox.corebanking.com/api/accounts/create
```

## 🔐 Security Best Practices

### 1. Use Environment Variables (Recommended)

Instead of storing secrets in `appsettings.json`, use environment variables:

```bash
# Set environment variables
export CoreBanking__ApiKey="your-api-key"
export CoreBanking__ApiSecret="your-api-secret"
export CoreBanking__ClientSecret="your-client-secret"
```

Or in `launchSettings.json`:
```json
{
  "environmentVariables": {
    "CoreBanking__ApiKey": "your-api-key",
    "CoreBanking__ApiSecret": "your-api-secret"
  }
}
```

### 2. Use Azure Key Vault (Production)

```csharp
// In Program.cs
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri("https://your-vault.vault.azure.net/"),
        new DefaultAzureCredential());
}
```

### 3. Never Commit Secrets

Add to `.gitignore`:
```
appsettings.Production.json
appsettings.*.Local.json
```

## 📊 Available Methods

### ICoreBankingService Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `CreateAccount(userId, accountType)` | Create a new bank account | `BankAccountDto` |
| `GetAccountBalance(accountNumber)` | Get current account balance | `decimal` |
| `Transfer(from, to, amount, narration)` | Perform fund transfer | `TransactionResultDto` |
| `GetTransactionHistory(accountNumber, skip, take)` | Get transaction history | `List<TransactionDto>` |
| `ValidateAccount(accountNumber, bankCode)` | Validate bank account | `BankAccountValidationDto` |

## 🔍 Logging

When `EnableLogging: true`, the service logs:
- Account creation attempts
- Balance inquiries
- Transfer requests
- Transaction history fetches
- Errors and exceptions

View logs in console or configure your logging provider.

## 🚨 Error Handling

The service throws exceptions on errors. Always wrap calls in try-catch:

```csharp
try
{
    var account = await _coreBankingService.CreateAccount(userId, "SAVINGS");
}
catch (HttpRequestException ex)
{
    // API connection error
    _logger.LogError(ex, "Failed to connect to Core Banking API");
}
catch (Exception ex)
{
    // Other errors
    _logger.LogError(ex, "Unexpected error in core banking operation");
}
```

## 🎯 Next Steps

1. **Update API credentials** in `appsettings.json`
2. **Create a BankingController** using the examples above
3. **Test with sandbox** before going to production
4. **Implement webhooks** for transaction notifications
5. **Add retry logic** for failed requests (Polly library recommended)
6. **Monitor API usage** and implement rate limiting

## 📞 Support

For Core Banking API support, contact your banking provider or check their documentation.

---

**Happy Banking Integration! 💰**


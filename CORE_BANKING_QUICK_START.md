# Core Banking - Quick Start

## ✅ What Was Created

### Configuration
- ✅ `Configuration/CoreBankingSettings.cs` - Strongly-typed settings class
- ✅ `appsettings.json` - Added CoreBanking section
- ✅ `appsettings.Development.json` - Development settings
- ✅ `Program.cs` - Registered configuration and service

### Services
- ✅ `Services/ICoreBankingService.cs` - Interface with 5 methods
- ✅ `Services/CoreBankingService.cs` - Full implementation with HttpClient

### Documentation
- ✅ `CORE_BANKING_GUIDE.md` - Complete guide
- ✅ `CORE_BANKING_QUICK_START.md` - This file

## 🚀 How to Use (3 Steps)

### Step 1: Update Your API Credentials

Edit `appsettings.json`:
```json
{
  "CoreBanking": {
    "BaseUrl": "https://your-banking-api.com",
    "ApiKey": "your-real-api-key",
    "ClientId": "your-real-client-id",
    "ClientSecret": "your-real-client-secret",
    "MerchantId": "your-merchant-id"
  }
}
```

### Step 2: Inject the Service

In any controller or service:
```csharp
public class MyController : ControllerBase
{
    private readonly ICoreBankingService _coreBanking;
    
    public MyController(ICoreBankingService coreBanking)
    {
        _coreBanking = coreBanking;
    }
    
    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount(int userId)
    {
        var account = await _coreBanking.CreateAccount(userId, "SAVINGS");
        return Ok(account);
    }
}
```

### Step 3: Start Using It!

```csharp
// Create account
var account = await _coreBanking.CreateAccount(userId, "SAVINGS");

// Check balance
var balance = await _coreBanking.GetAccountBalance("1234567890");

// Transfer money
var result = await _coreBanking.Transfer(
    fromAccount: "1234567890",
    toAccount: "0987654321",
    amount: 1000.00m,
    narration: "Payment for services"
);

// Get transaction history
var transactions = await _coreBanking.GetTransactionHistory("1234567890");

// Validate account
var validation = await _coreBanking.ValidateAccount("1234567890", "000");
```

## 📋 Configuration Properties

### Required (Must Configure)
- `BaseUrl` - Your banking API URL
- `ApiKey` - Your API key
- `ClientId` - Your client ID

### Optional (Has Defaults)
- `UseSandbox` - Use test environment (default: false)
- `TimeoutSeconds` - Request timeout (default: 30)
- `EnableLogging` - Log requests (default: true)
- `DefaultCurrency` - Currency code (default: "NGN")

## 🧪 Testing with Sandbox

For development/testing, use sandbox mode:

In `appsettings.Development.json`:
```json
{
  "CoreBanking": {
    "UseSandbox": true,
    "SandboxBaseUrl": "https://sandbox.your-bank.com",
    "EnableLogging": true
  }
}
```

The service will automatically use the sandbox URL when `UseSandbox: true`.

## 🔍 Access Configuration Values

If you need to access the settings directly:

```csharp
using Microsoft.Extensions.Options;

public class MyService
{
    private readonly CoreBankingSettings _settings;
    
    public MyService(IOptions<CoreBankingSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public void Example()
    {
        var url = _settings.GetEffectiveBaseUrl();
        var currency = _settings.DefaultCurrency;
        var isValid = _settings.IsValid();
    }
}
```

## 📖 Full Documentation

See `CORE_BANKING_GUIDE.md` for:
- Complete API reference
- Advanced examples
- Security best practices
- Error handling
- Testing strategies

## ⚡ Quick Example Controller

Create `Controllers/BankingController.cs`:

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
    private readonly ICoreBankingService _coreBanking;
    
    public BankingController(ICoreBankingService coreBanking)
    {
        _coreBanking = coreBanking;
    }
    
    [HttpPost("accounts")]
    public async Task<IActionResult> CreateAccount([FromBody] int userId)
    {
        var account = await _coreBanking.CreateAccount(userId, "SAVINGS");
        return Ok(account);
    }
    
    [HttpGet("accounts/{accountNumber}/balance")]
    public async Task<IActionResult> GetBalance(string accountNumber)
    {
        var balance = await _coreBanking.GetAccountBalance(accountNumber);
        return Ok(new { accountNumber, balance });
    }
}
```

## 🎯 Next Steps

1. ✅ **Update credentials** in `appsettings.json`
2. ✅ **Create a controller** (use example above)
3. ✅ **Test with Swagger** at `http://localhost:7001/swagger`
4. ✅ **Read full guide** in `CORE_BANKING_GUIDE.md`

---

**Ready to go! 🚀**


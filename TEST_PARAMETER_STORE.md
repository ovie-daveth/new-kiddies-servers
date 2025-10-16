# Test Parameter Store Connection

## ✅ Build Successful!

Your app is ready to test Parameter Store connection.

---

## 🧪 How to Test

### **Step 1: Create Your .env File**

Create a file named `.env` in the project root with:

```env
apiKey=71887f3a-0b9a-475a-8487-c5c5e6166898
baseUrl=http://192.168.207.10/ParameterStore
```

### **Step 2: Run the App**

```bash
dotnet run
```

### **Step 3: Watch the Startup Logs**

You'll see logs like:

```
🔌 Testing Parameter Store connection...
✅ Parameter Store connected successfully!
📡 Communications API: http://192.168.207.10/PremiumServices
📱 SMS Service API: http://192.168.207.10/PremiumServices
🏦 Core Enquiry API: https://dev.premiumtrustbank.com/middleware
📧 From Email configured: noreply@premiumtrustbank.com
🎉 CoreBanking services initialized successfully!
```

**OR if there's a problem:**

```
🔌 Testing Parameter Store connection...
❌ Failed to connect to Parameter Store!
⚠️  Authentication service will continue, but email features may not work
```

### **Step 4: Test the Endpoint**

Navigate to Swagger or use curl:

```bash
# Via Browser
http://localhost:7001/swagger

# Via curl
curl http://localhost:7001/api/auth/test-corebanking
```

**Expected Response (Success):**

```json
{
  "success": true,
  "message": "CoreBanking services connected successfully!",
  "parameterStore": {
    "connected": true,
    "parameterName": "AppSettings/CoreBanking"
  },
  "coreBanking": {
    "coreEnquiry": {
      "baseUrl": "https://dev.premiumtrustbank.com/middleware",
      "isEnabled": true,
      "apiKeyLength": 128
    },
    "communications": {
      "baseUrl": "http://192.168.207.10/PremiumServices",
      "endpoints": {
        "sendSMS": "api/Comms/SendSMS",
        "sendEmail": "api/Comms/SendEmail"
      }
    },
    "smsService": {
      "baseUrl": "http://192.168.207.10/PremiumServices",
      "apiKeyLength": 256
    },
    "authentication": {
      "baseUrl": "http://192.168.207.10/PremiumServices",
      "isEnabled": true
    }
  },
  "email": {
    "fromEmail": "noreply@premiumtrustbank.com"
  }
}
```

**Expected Response (Failure):**

```json
{
  "success": false,
  "message": "Failed to connect to CoreBanking services",
  "error": "Connection refused",
  "details": "Check your .env file and Parameter Store configuration"
}
```

---

## 🔍 What the App Does

### **On Startup (AuthService Constructor):**

1. Loads `.env` file ✅
2. Connects to Parameter Store at `http://192.168.207.10/ParameterStore` ✅
3. Fetches `AppSettings/CoreBanking` parameter ✅
4. Logs all settings to console ✅
5. Caches settings in memory (for fast access) ✅

### **API Call Made:**

```http
GET http://192.168.207.10/ParameterStore/api/parameters/AppSettings/CoreBanking
Headers:
  X-API-Key: 71887f3a-0b9a-475a-8487-c5c5e6166898
```

---

## 📊 Logs to Watch For

### **✅ Success Logs:**

```
info: ChatApp.Backend.Configuration.AppSettings[0]
      Fetching parameter from Parameter Store: AppSettings/CoreBanking
info: ChatApp.Backend.Configuration.AppSettings[0]
      Successfully fetched parameter: AppSettings/CoreBanking
info: ChatApp.Backend.Services.AuthService[0]
      🔌 Testing Parameter Store connection...
info: ChatApp.Backend.Services.AuthService[0]
      ✅ Parameter Store connected successfully!
info: ChatApp.Backend.Services.AuthService[0]
      📡 Communications API: http://192.168.207.10/PremiumServices
info: ChatApp.Backend.Services.AuthService[0]
      🎉 CoreBanking services initialized successfully!
```

### **❌ Failure Logs:**

```
info: ChatApp.Backend.Services.AuthService[0]
      🔌 Testing Parameter Store connection...
fail: ChatApp.Backend.Configuration.AppSettings[0]
      Error fetching parameter AppSettings/CoreBanking from Parameter Store
      System.Net.Http.HttpRequestException: Connection refused
warn: ChatApp.Backend.Services.AuthService[0]
      ⚠️  Authentication service will continue, but email features may not work
```

---

## 🐛 Troubleshooting

### Issue: "Connection Refused"

**Possible Causes:**
- Parameter Store API is down
- Wrong baseUrl in `.env`
- Network/firewall blocking connection

**Solution:**
```bash
# Test the URL directly
curl http://192.168.207.10/ParameterStore/api/parameters/AppSettings/CoreBanking \
  -H "X-API-Key: 71887f3a-0b9a-475a-8487-c5c5e6166898"
```

### Issue: "Unauthorized" or "403 Forbidden"

**Solution:**
Check your API key is correct in `.env` file.

### Issue: "Parameter Not Found" or "404"

**Possible Causes:**
- Parameter name mismatch
- API endpoint format different

**Solution:**
1. Verify parameter exists: `AppSettings/CoreBanking`
2. Check API endpoint format in `Configuration/AppSettings.cs` (line 55)

### Issue: No Logs Showing

**Solution:**
Check your logging level in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "ChatApp.Backend": "Information"
    }
  }
}
```

---

## 🎯 Next Steps (After Testing)

### **If Test Succeeds ✅:**

1. Your Parameter Store is connected!
2. You can now install CoreBankingService NuGet package:
   ```bash
   # First, update nuget.config with your NuGet feed URL
   # Then install:
   dotnet add package CoreBankingService
   ```

3. Uncomment the CoreBankingService references in `AuthService.cs` (lines 10-12, 21, 30, 36)

4. Use `ICommunicationService` to send emails:
   ```csharp
   await _communicationService.SendEmail(new SendEmailRequest
   {
       fromEmail = _fromEmail,
       toEmail = user.Email,
       subject = "Welcome!",
       body = html
   });
   ```

### **If Test Fails ❌:**

1. Check your `.env` file exists and has correct values
2. Verify Parameter Store API is accessible
3. Test with curl/Postman first
4. Check network connectivity
5. Verify API key permissions

---

## 📝 Test Checklist

- [ ] Created `.env` file with `apiKey` and `baseUrl`
- [ ] Run `dotnet run`
- [ ] Check startup logs for connection test
- [ ] Visit `/api/auth/test-corebanking` endpoint
- [ ] Verify JSON response shows all settings
- [ ] Check logs show successful connection

---

## 🔗 Test Endpoints

| Endpoint | Purpose |
|----------|---------|
| `GET /api/auth/test-corebanking` | Test Parameter Store connection |
| `GET /health` | Test app is running |
| `GET /swagger` | API documentation |

---

**Run the app now and watch the logs to see if Parameter Store connects! 🚀**

```bash
dotnet run
```


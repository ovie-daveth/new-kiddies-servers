# .env File Setup Guide

## ✅ Setup Complete!

Your application is now configured to read sensitive configuration from a `.env` file instead of `appsettings.json`.

---

## 🚀 How to Set Up Your .env File

### Step 1: Copy the Template

Copy `env.template` to `.env`:

```powershell
# Windows PowerShell
Copy-Item env.template .env
```

```bash
# Linux/Mac
cp env.template .env
```

### Step 2: Edit .env File

Open the `.env` file and add your actual values:

```env
# Parameter Store Configuration
PARAMETER_STORE_BASE_URL=https://your-actual-parameter-store-api.premiumtrustbank.com
PARAMETER_STORE_API_KEY=your-real-api-key-here
```

**Replace with your actual values:**
- `PARAMETER_STORE_BASE_URL` - Your company's Parameter Store API URL
- `PARAMETER_STORE_API_KEY` - Your API key

### Step 3: Save the File

Save `.env` file in the **root** of your project (same folder as `Program.cs`).

---

## 📁 File Structure

```
your-project/
├── .env                    ← Your actual secrets (NEVER commit this!)
├── env.template            ← Template for team members
├── .gitignore              ← Updated to exclude .env
├── Program.cs              ← Loads .env automatically
├── appsettings.json        ← No secrets here anymore
└── ...
```

---

## 🔒 Security - IMPORTANT!

### ✅ DO:
- ✅ Keep `.env` file **LOCAL ONLY**
- ✅ Share `env.template` with your team
- ✅ Add different values for dev/staging/production
- ✅ Verify `.env` is in `.gitignore`

### ❌ DON'T:
- ❌ **NEVER** commit `.env` to git
- ❌ **NEVER** share `.env` file publicly
- ❌ **NEVER** include real secrets in `env.template`

---

## 🧪 Test It Works

### Step 1: Create Your .env File

```env
PARAMETER_STORE_BASE_URL=https://your-api.com
PARAMETER_STORE_API_KEY=test-key-123
```

### Step 2: Run the App

```bash
dotnet run
```

### Step 3: Check the Logs

You should see environment variables loaded successfully.

---

## 📖 Available Environment Variables

You can add any of these to your `.env` file:

### Required:
```env
PARAMETER_STORE_BASE_URL=https://your-api.com
PARAMETER_STORE_API_KEY=your-api-key
```

### Optional (if you want to move more secrets):
```env
# Database
CONNECTION_STRING=Server=...;Database=...;

# JWT
JWT_SECRET_KEY=your-secret-key
JWT_ISSUER=ChatApp.Backend
JWT_AUDIENCE=ChatApp.Client

# AWS (if needed)
AWS_ACCESS_KEY_ID=your-key
AWS_SECRET_ACCESS_KEY=your-secret
AWS_REGION=us-east-1
```

---

## 🔄 How It Works

1. **App starts** → `Program.cs` loads `.env` file (line 11)
2. **Environment variables** are set from `.env` file
3. **Configuration** is overridden with env vars (lines 16-17)
4. **AppSettings** reads from configuration
5. **Parameter Store API** is called with your credentials

---

## 💻 Example .env File

```env
# Parameter Store Configuration
PARAMETER_STORE_BASE_URL=https://paramstore.premiumtrustbank.com
PARAMETER_STORE_API_KEY=ptb_live_abc123def456ghi789

# Optional: Database override
# CONNECTION_STRING=Server=production-db.com;Database=MyDb;User=sa;Password=SecurePass123!

# Optional: JWT override
# JWT_SECRET_KEY=super-secret-production-key-change-this-12345678901234567890
```

---

## 🌍 Different Environments

### Development (.env)
```env
PARAMETER_STORE_BASE_URL=https://dev.paramstore.premiumtrustbank.com
PARAMETER_STORE_API_KEY=dev_key_123
```

### Staging (.env.staging)
```env
PARAMETER_STORE_BASE_URL=https://staging.paramstore.premiumtrustbank.com
PARAMETER_STORE_API_KEY=staging_key_456
```

### Production (.env.production)
```env
PARAMETER_STORE_BASE_URL=https://prod.paramstore.premiumtrustbank.com
PARAMETER_STORE_API_KEY=prod_key_789
```

**Load specific env file:**
```csharp
// In Program.cs, change line 11 to:
DotNetEnv.Env.Load(".env.production");
```

---

## 🐛 Troubleshooting

### Issue: "Environment variable not found"

**Solution:**
1. Check `.env` file exists in project root
2. Check variable names match exactly (case-sensitive)
3. Restart your IDE/terminal

### Issue: ".env file not loading"

**Solution:**
1. Verify file is named exactly `.env` (not `.env.txt`)
2. Check file is in the same folder as `Program.cs`
3. Look for errors in startup logs

### Issue: "Still using old values"

**Solution:**
1. Stop the application completely
2. Rebuild: `dotnet clean && dotnet build`
3. Run again: `dotnet run`

---

## ✅ Verification Checklist

- [ ] Created `.env` file from `env.template`
- [ ] Added real `PARAMETER_STORE_BASE_URL`
- [ ] Added real `PARAMETER_STORE_API_KEY`
- [ ] Verified `.env` is in `.gitignore`
- [ ] Tested app runs: `dotnet run`
- [ ] Confirmed values are loaded (check logs)

---

## 📞 Get Your Credentials

Ask your DevOps/Infrastructure team for:
- ✅ Parameter Store API Base URL
- ✅ API Key for your environment (dev/staging/prod)

---

## 🎯 Quick Start Commands

```bash
# 1. Copy template
cp env.template .env

# 2. Edit .env file (add your values)
# Use your favorite editor: nano, vim, code, notepad, etc.

# 3. Verify .env is not tracked by git
git status
# Should NOT show .env file

# 4. Run the app
dotnet run

# 5. Test Parameter Store connection
curl http://localhost:7001/api/testparameterstore/test
```

---

**Your secrets are now safe in .env file! 🔒**

Remember: **NEVER commit .env to git!**


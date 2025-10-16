# 🚀 Deployment Guide - Kids Social Media Platform

Your application is now ready for deployment to Azure! This guide provides everything you need.

## 📋 Quick Reference

| Deployment Method | Time | Difficulty | Best For |
|-------------------|------|------------|----------|
| **PowerShell Script** | 5-10 min | Easy | First-time deployment |
| **Visual Studio** | 3-5 min | Very Easy | Quick deployment |
| **GitHub Actions** | 10-15 min setup | Medium | CI/CD pipeline |
| **Docker** | 5-10 min | Medium | Container deployment |
| **Azure Portal** | 15-20 min | Medium | Manual control |

## 🎯 Recommended: PowerShell Script (Fastest)

### One-Command Deployment:
```powershell
.\deploy-to-azure.ps1 -SqlAdminPassword "YourStrongPassword123!"
```

That's it! Everything else is automated.

### What It Does:
- ✅ Creates Azure Resource Group
- ✅ Sets up App Service and SQL Database
- ✅ Configures all settings automatically
- ✅ Builds and deploys your application
- ✅ Enables WebSockets for SignalR

### After Deployment:
1. **Run migrations** (copy from script output)
2. **Test your API** at `https://kiddies-social-api.azurewebsites.net/swagger`
3. **Update frontend** with new API URL

## 📚 Documentation Files

- **`AZURE_QUICK_START.md`** - Fast deployment in 10 minutes
- **`AZURE_DEPLOYMENT_GUIDE.md`** - Complete detailed guide
- **`deploy-to-azure.ps1`** - Automated deployment script
- **`docker-compose.yml`** - Local Docker testing
- **`Dockerfile`** - Container deployment

## 🔧 Deployment Options Detailed

### Option 1: PowerShell Script ⭐ RECOMMENDED
```powershell
# Basic deployment
.\deploy-to-azure.ps1 -SqlAdminPassword "YourPassword123!"

# Custom configuration
.\deploy-to-azure.ps1 `
    -WebAppName "my-custom-name" `
    -Location "westus" `
    -SqlAdminPassword "YourPassword123!"
```

### Option 2: Visual Studio (Easiest)
1. Right-click project → **Publish**
2. Choose **Azure** → **App Service**
3. Sign in and create new
4. Click **Publish**

### Option 3: GitHub Actions (CI/CD)
1. Run deployment script first (to create Azure resources)
2. Get publish profile from Azure Portal
3. Add to GitHub Secrets as `AZURE_WEBAPP_PUBLISH_PROFILE`
4. Push to main branch - auto-deploys!

### Option 4: Docker Containers
```bash
# Local testing
docker-compose up -d

# Build for Azure
docker build -t kiddies-social-api .
docker tag kiddies-social-api yourregistry.azurecr.io/kiddies-social-api
docker push yourregistry.azurecr.io/kiddies-social-api
```

## 🌐 Your Application URLs

After deployment, your app will be available at:

| Endpoint | URL |
|----------|-----|
| **API** | `https://kiddies-social-api.azurewebsites.net` |
| **Swagger** | `https://kiddies-social-api.azurewebsites.net/swagger` |
| **Health Check** | `https://kiddies-social-api.azurewebsites.net/health` |
| **Chat Hub** | `https://kiddies-social-api.azurewebsites.net/hubs/chat` |
| **Notification Hub** | `https://kiddies-social-api.azurewebsites.net/hubs/notification` |
| **Post Hub** | `https://kiddies-social-api.azurewebsites.net/hubs/post` |

## ⚙️ Post-Deployment Configuration

### 1. Run Database Migrations
```powershell
dotnet ef database update --connection "Server=tcp:kiddies-social-sql.database.windows.net,1433;Initial Catalog=KidsSocialMediaDb;User ID=sqladmin;Password=YourPassword123!;Encrypt=True;"
```

### 2. Update CORS in Program.cs
Uncomment and update the Azure URL in `Program.cs` (lines 116):
```csharp
policy.WithOrigins(
    "http://localhost:3000", 
    "http://localhost:4200",
    "https://kiddies-social-api.azurewebsites.net"  // ← Uncomment this
)
```

### 3. Update Frontend
Point your frontend to the new Azure API URL.

## 🔒 Security Checklist

Before going to production:

- [ ] Change JWT SecretKey to a unique, strong value
- [ ] Store secrets in Azure Key Vault (not appsettings.json)
- [ ] Update CORS to only allow your frontend domains
- [ ] Enable Azure Application Insights for monitoring
- [ ] Configure Azure SQL firewall rules properly
- [ ] Enable HTTPS only (already configured)
- [ ] Set up automated backups for SQL Database
- [ ] Implement rate limiting
- [ ] Review and test all authentication endpoints

## 💰 Cost Breakdown

### Development/Testing (~$18/month)
- App Service (B1): $13/month
- Azure SQL (Basic): $5/month

### Production (~$141/month)
- App Service (P1V2): $96/month
- Azure SQL (S2): $30/month
- Application Insights: $10/month
- Azure Blob Storage: $5/month

### Free Tier (Testing Only)
- App Service (F1): Free
- Azure SQL (5GB): $5/month
- **Limitations**: No custom domains, limited compute

## 🧪 Testing Your Deployment

### 1. Health Check
```bash
curl https://kiddies-social-api.azurewebsites.net/health
```

### 2. Swagger UI
Visit: `https://kiddies-social-api.azurewebsites.net/swagger`

### 3. Test Authentication
```bash
# Register a user
POST https://kiddies-social-api.azurewebsites.net/api/auth/register

# Login
POST https://kiddies-social-api.azurewebsites.net/api/auth/login
```

### 4. Test SignalR Hubs
Use SignalR client to connect to:
- `https://kiddies-social-api.azurewebsites.net/hubs/chat`

## 🐛 Troubleshooting

### App Not Starting
```bash
# View logs
az webapp log tail --name kiddies-social-api --resource-group kiddies-social-rg
```

### Database Connection Failed
1. Check connection string in Azure Portal
2. Verify SQL Server firewall rules
3. Enable "Allow Azure Services"

### SignalR Not Working
1. Ensure WebSockets are enabled (script does this automatically)
2. Check CORS configuration
3. Verify JWT token is passed correctly

### File Upload Issues
- Move to Azure Blob Storage for production
- Check file size limits in Azure App Service

## 📊 Monitoring & Logs

### View Application Logs
```bash
# Stream logs
az webapp log tail --name kiddies-social-api --resource-group kiddies-social-rg

# Download logs
az webapp log download --name kiddies-social-api --resource-group kiddies-social-rg
```

### Access Kudu Console
Navigate to: `https://kiddies-social-api.scm.azurewebsites.net`

### Enable Application Insights
```bash
az monitor app-insights component create \
    --app kiddies-social-insights \
    --location eastus \
    --resource-group kiddies-social-rg
```

## 🚀 Production Best Practices

### 1. Use Azure Key Vault for Secrets
```bash
az keyvault create --name kiddies-social-vault --resource-group kiddies-social-rg
az keyvault secret set --vault-name kiddies-social-vault --name JwtSecretKey --value "your-secret"
```

### 2. Enable Auto-Scaling
```bash
az monitor autoscale create \
    --resource-group kiddies-social-rg \
    --resource kiddies-social-api \
    --min-count 1 \
    --max-count 3 \
    --count 1
```

### 3. Use Azure CDN for Static Files
- Configure Azure Blob Storage for uploads
- Set up Azure CDN for media delivery
- Improves performance globally

### 4. Database Backups
```bash
# Enable automated backups (already enabled by default)
az sql db show --name KidsSocialMediaDb \
    --server kiddies-social-sql \
    --resource-group kiddies-social-rg
```

### 5. SignalR Scale-Out with Redis
For multiple instances:
```bash
# Create Redis Cache
az redis create --name kiddies-social-redis \
    --resource-group kiddies-social-rg \
    --location eastus \
    --sku Basic --vm-size c0
```

## 🔄 CI/CD with GitHub Actions

Your workflow is already configured in `.github/workflows/azure-deploy.yml`

### Setup Steps:
1. Get publish profile:
   ```bash
   az webapp deployment list-publishing-profiles \
       --name kiddies-social-api \
       --resource-group kiddies-social-rg \
       --xml
   ```

2. Add to GitHub Secrets:
   - Go to repo Settings → Secrets
   - Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Value: Paste the XML

3. Push to main branch → Auto-deploys!

## 📞 Support & Resources

- **Azure Portal**: https://portal.azure.com
- **Documentation**: See `AZURE_DEPLOYMENT_GUIDE.md`
- **Quick Start**: See `AZURE_QUICK_START.md`
- **ASP.NET Core on Azure**: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/
- **SignalR on Azure**: https://docs.microsoft.com/en-us/aspnet/core/signalr/scale

## 🎉 You're Ready!

Choose your deployment method and get started:

1. **Quick & Easy**: Use the PowerShell script
2. **CI/CD**: Set up GitHub Actions
3. **Container**: Use Docker
4. **Manual**: Follow detailed guide

Good luck with your deployment! 🚀


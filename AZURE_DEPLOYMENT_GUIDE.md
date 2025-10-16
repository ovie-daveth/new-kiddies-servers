# Azure Deployment Guide

This guide will help you deploy your Kids Social Media Platform API to Azure.

## Prerequisites

1. **Azure Account**: Sign up at [https://azure.microsoft.com](https://azure.microsoft.com)
2. **Azure CLI**: Install from [https://docs.microsoft.com/en-us/cli/azure/install-azure-cli](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
3. **.NET 8 SDK**: Already installed (you're using it)

## Deployment Options

### Option 1: Deploy via Visual Studio (Easiest)

1. **Right-click** on the `ChatApp.Backend` project in Solution Explorer
2. Select **Publish**
3. Choose **Azure** as the target
4. Select **Azure App Service (Windows)** or **(Linux)**
5. Sign in to your Azure account
6. Click **Create New** to create a new App Service
7. Fill in the details:
   - **Name**: `kiddies-social-api` (or your preferred name)
   - **Subscription**: Select your Azure subscription
   - **Resource Group**: Create new or select existing
   - **Hosting Plan**: Create new (choose appropriate size/region)
8. Click **Create** and then **Publish**

### Option 2: Deploy via Azure CLI

#### Step 1: Login to Azure
```bash
az login
```

#### Step 2: Create Resource Group
```bash
az group create --name kiddies-social-rg --location eastus
```

#### Step 3: Create App Service Plan
```bash
az appservice plan create --name kiddies-social-plan --resource-group kiddies-social-rg --sku B1 --is-linux
```

#### Step 4: Create Web App
```bash
az webapp create --name kiddies-social-api --resource-group kiddies-social-rg --plan kiddies-social-plan --runtime "DOTNET|8.0"
```

#### Step 5: Create Azure SQL Database
```bash
# Create SQL Server
az sql server create --name kiddies-social-sql --resource-group kiddies-social-rg --location eastus --admin-user sqladmin --admin-password "YourStrongPassword123!"

# Create Database
az sql db create --name KidsSocialMediaDb --server kiddies-social-sql --resource-group kiddies-social-rg --service-objective S0

# Configure Firewall (Allow Azure Services)
az sql server firewall-rule create --resource-group kiddies-social-rg --server kiddies-social-sql --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
```

#### Step 6: Configure App Settings
```bash
# Get SQL Connection String
$connectionString = "Server=tcp:kiddies-social-sql.database.windows.net,1433;Initial Catalog=KidsSocialMediaDb;Persist Security Info=False;User ID=sqladmin;Password=YourStrongPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Set Connection String
az webapp config connection-string set --name kiddies-social-api --resource-group kiddies-social-rg --connection-string-type SQLAzure --settings DefaultConnection=$connectionString

# Set App Settings (JWT Configuration)
az webapp config appsettings set --name kiddies-social-api --resource-group kiddies-social-rg --settings `
  JwtSettings__SecretKey="YourSuperSecretKeyHere_MinimumLength32Characters_ChangeThisInProduction!" `
  JwtSettings__Issuer="ChatApp.Backend" `
  JwtSettings__Audience="ChatApp.Client" `
  ASPNETCORE_ENVIRONMENT="Production"
```

#### Step 7: Build and Deploy
```bash
# Build the application
dotnet publish -c Release -o ./publish

# Create a zip file
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force

# Deploy to Azure
az webapp deployment source config-zip --name kiddies-social-api --resource-group kiddies-social-rg --src ./app.zip
```

### Option 3: Deploy via GitHub Actions (CI/CD)

This is the recommended approach for continuous deployment.

#### Step 1: Get Publish Profile
```bash
az webapp deployment list-publishing-profiles --name kiddies-social-api --resource-group kiddies-social-rg --xml
```

#### Step 2: Add to GitHub Secrets
1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
5. Value: Paste the XML from Step 1

#### Step 3: Create GitHub Actions Workflow
I'll create this file for you in the next step.

## Post-Deployment Configuration

### 1. Update CORS Settings
Update your `Program.cs` to include your Azure URL:

```csharp
policy.WithOrigins(
    "http://localhost:3000", 
    "http://localhost:4200",
    "https://kiddies-social-api.azurewebsites.net",  // Add your Azure URL
    "https://your-frontend-domain.com"  // Add your frontend URL
)
```

### 2. Run Database Migrations
After deploying, you need to apply migrations to your Azure SQL Database:

**Option A: Using Azure Cloud Shell**
```bash
dotnet ef database update --connection "Your-Azure-SQL-Connection-String"
```

**Option B: Using Visual Studio Package Manager Console**
```powershell
Update-Database -Connection "Your-Azure-SQL-Connection-String"
```

**Option C: Using EF Core Bundle** (Recommended)
```bash
# Create a bundle
dotnet ef migrations bundle --self-contained -r win-x64

# Upload and run on Azure via Kudu console
./efbundle.exe --connection "Your-Azure-SQL-Connection-String"
```

### 3. Configure Static Files
Your app uses static files (uploaded images/videos). In Azure, these will be stored in the wwwroot folder by default. For production, consider:

- **Azure Blob Storage** (Recommended for media files)
- **Azure CDN** for better performance

### 4. Enable WebSockets (for SignalR)
```bash
az webapp config set --name kiddies-social-api --resource-group kiddies-social-rg --web-sockets-enabled true
```

### 5. Scale Settings (Optional)
```bash
# Scale up (more powerful instance)
az appservice plan update --name kiddies-social-plan --resource-group kiddies-social-rg --sku P1V2

# Scale out (more instances)
az webapp scale --name kiddies-social-api --resource-group kiddies-social-rg --instance-count 2
```

## Important Production Considerations

### 1. Security
- ✅ Change JWT SecretKey to a strong, unique value
- ✅ Store secrets in Azure Key Vault (not appsettings.json)
- ✅ Enable HTTPS only
- ✅ Configure proper CORS origins
- ✅ Implement rate limiting
- ✅ Enable Azure Application Insights for monitoring

### 2. Database
- ✅ Enable automated backups for Azure SQL
- ✅ Configure firewall rules properly
- ✅ Use connection string from Azure portal
- ✅ Consider geo-replication for high availability

### 3. File Storage
- ✅ Move from local file storage to Azure Blob Storage
- ✅ Configure Azure CDN for media delivery
- ✅ Implement proper file size limits and validation

### 4. Performance
- ✅ Enable response caching where appropriate
- ✅ Configure Application Insights for monitoring
- ✅ Set up auto-scaling rules
- ✅ Use Azure Redis Cache for SignalR backplane (if scaling out)

## Testing Your Deployment

1. **Check if the app is running:**
   ```
   https://kiddies-social-api.azurewebsites.net
   ```

2. **Access Swagger:**
   ```
   https://kiddies-social-api.azurewebsites.net/swagger
   ```

3. **Test SignalR Hubs:**
   ```
   https://kiddies-social-api.azurewebsites.net/hubs/chat
   https://kiddies-social-api.azurewebsites.net/hubs/notification
   https://kiddies-social-api.azurewebsites.net/hubs/post
   ```

## Troubleshooting

### View Application Logs
```bash
az webapp log tail --name kiddies-social-api --resource-group kiddies-social-rg
```

### Access Kudu Console
Navigate to: `https://kiddies-social-api.scm.azurewebsites.net`

### Common Issues

1. **Database Connection Failed**
   - Check connection string in Azure App Configuration
   - Verify SQL Server firewall rules
   - Ensure "Allow Azure Services" is enabled

2. **SignalR Not Working**
   - Enable WebSockets in Azure App Service
   - Check CORS configuration
   - Verify authentication token is being passed

3. **File Upload Issues**
   - Check wwwroot permissions
   - Consider moving to Azure Blob Storage
   - Verify file size limits in App Service

## Cost Estimation

**Basic Setup (Development/Testing):**
- App Service (B1): ~$13/month
- Azure SQL (Basic): ~$5/month
- **Total: ~$18/month**

**Production Setup:**
- App Service (P1V2): ~$96/month
- Azure SQL (S2): ~$30/month
- Application Insights: ~$10/month
- Azure Blob Storage: ~$5/month
- **Total: ~$141/month**

## Next Steps

1. Choose your deployment method (Visual Studio is easiest for first time)
2. Create Azure resources (Resource Group, App Service, SQL Database)
3. Configure app settings and connection strings
4. Deploy your application
5. Run database migrations
6. Test all endpoints and SignalR hubs
7. Update your frontend to use the new Azure URL

## Useful Azure CLI Commands

```bash
# List all resources in resource group
az resource list --resource-group kiddies-social-rg --output table

# Restart web app
az webapp restart --name kiddies-social-api --resource-group kiddies-social-rg

# View app service logs
az webapp log download --name kiddies-social-api --resource-group kiddies-social-rg

# Delete resource group (removes all resources)
az group delete --name kiddies-social-rg --yes
```

## Support Links

- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Azure SQL Database Documentation](https://docs.microsoft.com/en-us/azure/sql-database/)
- [Deploy ASP.NET Core to Azure](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/)
- [SignalR on Azure](https://docs.microsoft.com/en-us/aspnet/core/signalr/scale)


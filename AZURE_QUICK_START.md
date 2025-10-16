# Azure Deployment - Quick Start

This guide will help you deploy to Azure in under 10 minutes.

## Method 1: Using PowerShell Script (Recommended - Fastest)

### Prerequisites
- Azure CLI installed
- Azure account

### Steps

1. **Open PowerShell in the project directory**

2. **Run the deployment script:**
   ```powershell
   .\deploy-to-azure.ps1 -SqlAdminPassword "YourStrongPassword123!"
   ```

   That's it! The script will:
   - Create all Azure resources
   - Configure settings
   - Build and deploy your app

3. **Run Database Migrations:**
   After deployment, run:
   ```powershell
   dotnet ef database update --connection "Server=tcp:kiddies-social-sql.database.windows.net,1433;Initial Catalog=KidsSocialMediaDb;User ID=sqladmin;Password=YourStrongPassword123!;Encrypt=True;"
   ```

4. **Access Your App:**
   - API: `https://kiddies-social-api.azurewebsites.net`
   - Swagger: `https://kiddies-social-api.azurewebsites.net/swagger`

### Custom Configuration

You can customize the deployment:

```powershell
.\deploy-to-azure.ps1 `
    -ResourceGroupName "my-custom-rg" `
    -WebAppName "my-api-name" `
    -SqlServerName "my-sql-server" `
    -Location "westus" `
    -SqlAdminPassword "YourStrongPassword123!" `
    -JwtSecretKey "YourCustomSecretKey123456789012345678901234567890"
```

## Method 2: Using Visual Studio (Easiest)

1. **Right-click** the project → **Publish**
2. **Select** Azure → Azure App Service (Windows/Linux)
3. **Sign in** to Azure
4. **Create new** App Service
5. **Click Publish**
6. Done! Visual Studio handles everything.

## Method 3: Using Azure Portal (Manual)

1. **Create App Service:**
   - Go to [Azure Portal](https://portal.azure.com)
   - Create Resource → Web App
   - Runtime: .NET 8
   - Region: Choose closest to you

2. **Create SQL Database:**
   - Create Resource → SQL Database
   - Configure server and database

3. **Configure App Settings:**
   - Go to Web App → Configuration
   - Add Connection String: `DefaultConnection`
   - Add App Settings:
     - `JwtSettings__SecretKey`
     - `JwtSettings__Issuer`
     - `JwtSettings__Audience`
     - `ASPNETCORE_ENVIRONMENT=Production`

4. **Deploy:**
   ```powershell
   dotnet publish -c Release -o ./publish
   Compress-Archive -Path ./publish/* -DestinationPath ./app.zip
   az webapp deployment source config-zip --name YOUR_APP_NAME --resource-group YOUR_RG --src ./app.zip
   ```

## Method 4: GitHub Actions (CI/CD)

1. **Get Publish Profile:**
   - Go to your App Service in Azure Portal
   - Click "Get publish profile"
   - Copy the XML content

2. **Add to GitHub Secrets:**
   - Go to your GitHub repo → Settings → Secrets
   - New secret: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Paste the XML

3. **Update `.github/workflows/azure-deploy.yml`:**
   - Change `AZURE_WEBAPP_NAME` to your app name
   - Push to main branch
   - GitHub Actions will automatically deploy!

## Post-Deployment Checklist

- [ ] Run database migrations
- [ ] Test API at `https://YOUR-APP.azurewebsites.net/swagger`
- [ ] Test SignalR hubs
- [ ] Update frontend with new API URL
- [ ] Enable Application Insights (monitoring)
- [ ] Configure custom domain (optional)
- [ ] Set up CI/CD with GitHub Actions
- [ ] Configure Azure CDN (optional, for better performance)

## Troubleshooting

### App Not Starting
```bash
# View logs
az webapp log tail --name kiddies-social-api --resource-group kiddies-social-rg
```

### Database Connection Issues
- Check connection string in Azure Portal
- Verify SQL firewall allows Azure services
- Test connection using SQL Server Management Studio

### SignalR Not Working
- Ensure WebSockets are enabled in Azure App Service
- Check CORS configuration includes your Azure URL
- Verify authentication is working

## Cost Optimization

**Free Tier (for testing):**
```powershell
az appservice plan create --name kiddies-social-plan --resource-group kiddies-social-rg --sku F1 --is-linux
```
Note: Free tier has limitations (no custom domains, limited resources)

**Basic Tier (recommended for development):**
```powershell
az appservice plan create --name kiddies-social-plan --resource-group kiddies-social-rg --sku B1 --is-linux
```
Cost: ~$13/month + ~$5 for database = ~$18/month

## Useful Commands

```powershell
# Restart app
az webapp restart --name kiddies-social-api --resource-group kiddies-social-rg

# View logs
az webapp log tail --name kiddies-social-api --resource-group kiddies-social-rg

# List all resources
az resource list --resource-group kiddies-social-rg --output table

# Delete everything (be careful!)
az group delete --name kiddies-social-rg --yes
```

## Support

- Full guide: See `AZURE_DEPLOYMENT_GUIDE.md`
- Azure Support: [Azure Portal](https://portal.azure.com) → Help + support
- Documentation: [ASP.NET Core on Azure](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/)


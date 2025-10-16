# Azure Deployment - Cheat Sheet

## 🚀 Quick Deploy Command
```powershell
.\deploy-to-azure.ps1 -SqlAdminPassword "YourStrongPassword123!"
```

## 📋 Essential Commands

### Deploy Application
```powershell
# Build and publish
dotnet publish -c Release -o ./publish

# Create zip
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force

# Deploy
az webapp deployment source config-zip \
    --name kiddies-social-api \
    --resource-group kiddies-social-rg \
    --src ./app.zip
```

### Database Migrations
```powershell
# Run migrations
dotnet ef database update --connection "YOUR_AZURE_SQL_CONNECTION_STRING"

# Create new migration
dotnet ef migrations add MigrationName

# Create migration bundle
dotnet ef migrations bundle --self-contained -r win-x64
```

### View Logs
```bash
# Stream logs
az webapp log tail --name kiddies-social-api --resource-group kiddies-social-rg

# Download logs
az webapp log download --name kiddies-social-api --resource-group kiddies-social-rg
```

### Restart Application
```bash
az webapp restart --name kiddies-social-api --resource-group kiddies-social-rg
```

### Configuration
```bash
# Set app settings
az webapp config appsettings set \
    --name kiddies-social-api \
    --resource-group kiddies-social-rg \
    --settings KEY=VALUE

# Set connection string
az webapp config connection-string set \
    --name kiddies-social-api \
    --resource-group kiddies-social-rg \
    --connection-string-type SQLAzure \
    --settings DefaultConnection="YOUR_CONNECTION_STRING"

# Enable WebSockets
az webapp config set \
    --name kiddies-social-api \
    --resource-group kiddies-social-rg \
    --web-sockets-enabled true
```

## 🌐 Your URLs

| Service | URL |
|---------|-----|
| API | `https://kiddies-social-api.azurewebsites.net` |
| Swagger | `https://kiddies-social-api.azurewebsites.net/swagger` |
| Health | `https://kiddies-social-api.azurewebsites.net/health` |
| Kudu | `https://kiddies-social-api.scm.azurewebsites.net` |

## 🔧 Resource Management

```bash
# List resources
az resource list --resource-group kiddies-social-rg --output table

# Scale up (more power)
az appservice plan update \
    --name kiddies-social-plan \
    --resource-group kiddies-social-rg \
    --sku P1V2

# Scale out (more instances)
az webapp scale \
    --name kiddies-social-api \
    --resource-group kiddies-social-rg \
    --instance-count 2

# Delete resource group (CAREFUL!)
az group delete --name kiddies-social-rg --yes
```

## 🐳 Docker Commands

```bash
# Build image
docker build -t kiddies-social-api .

# Run locally
docker run -p 5000:80 kiddies-social-api

# Run with docker-compose
docker-compose up -d

# View logs
docker-compose logs -f

# Stop
docker-compose down
```

## 🔍 SQL Database

```bash
# Connection string format
Server=tcp:kiddies-social-sql.database.windows.net,1433;Initial Catalog=KidsSocialMediaDb;User ID=sqladmin;Password=YOUR_PASSWORD;Encrypt=True;

# Create firewall rule for your IP
az sql server firewall-rule create \
    --resource-group kiddies-social-rg \
    --server kiddies-social-sql \
    --name MyIP \
    --start-ip-address YOUR_IP \
    --end-ip-address YOUR_IP

# List databases
az sql db list \
    --server kiddies-social-sql \
    --resource-group kiddies-social-rg \
    --output table
```

## 🔐 Security

```bash
# View app settings (without values)
az webapp config appsettings list \
    --name kiddies-social-api \
    --resource-group kiddies-social-rg

# Enable HTTPS only
az webapp update \
    --name kiddies-social-api \
    --resource-group kiddies-social-rg \
    --https-only true

# Enable managed identity
az webapp identity assign \
    --name kiddies-social-api \
    --resource-group kiddies-social-rg
```

## 📊 Monitoring

```bash
# Enable Application Insights
az monitor app-insights component create \
    --app kiddies-social-insights \
    --location eastus \
    --resource-group kiddies-social-rg

# View metrics
az monitor metrics list \
    --resource kiddies-social-api \
    --resource-group kiddies-social-rg \
    --resource-type "Microsoft.Web/sites"
```

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| App won't start | Check logs: `az webapp log tail` |
| Database connection failed | Verify connection string and firewall rules |
| SignalR not working | Enable WebSockets in App Service |
| 502 Bad Gateway | App is starting, wait 1-2 minutes |
| File upload fails | Check file size limits and permissions |

## 📱 Test Endpoints

```bash
# Health check
curl https://kiddies-social-api.azurewebsites.net/health

# Register user
curl -X POST https://kiddies-social-api.azurewebsites.net/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"test","email":"test@test.com","password":"Test123!"}'

# Login
curl -X POST https://kiddies-social-api.azurewebsites.net/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"Test123!"}'
```

## 💡 Tips

- **Always test locally first**: `dotnet run`
- **Use staging slots**: For zero-downtime deployments
- **Monitor costs**: Set up cost alerts in Azure Portal
- **Backup database**: Enable automated backups
- **Use Key Vault**: For production secrets
- **Enable auto-scaling**: For variable loads
- **Use CDN**: For static files and media
- **Test SignalR**: Ensure WebSockets are enabled

## 📚 Quick Links

- Azure Portal: https://portal.azure.com
- Kudu Console: https://kiddies-social-api.scm.azurewebsites.net
- Swagger: https://kiddies-social-api.azurewebsites.net/swagger
- Documentation: See `DEPLOYMENT_README.md`


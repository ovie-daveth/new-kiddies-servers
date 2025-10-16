# Azure Deployment Script for Kids Social Media Platform
# This script helps you deploy the application to Azure App Service

param(
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroupName = "kiddies-social-rg",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "eastus",
    
    [Parameter(Mandatory=$false)]
    [string]$AppServicePlanName = "kiddies-social-plan",
    
    [Parameter(Mandatory=$false)]
    [string]$WebAppName = "kiddies-social-api",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlServerName = "kiddies-social-sql",
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseName = "KidsSocialMediaDb",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlAdminUser = "sqladmin",
    
    [Parameter(Mandatory=$true)]
    [string]$SqlAdminPassword,
    
    [Parameter(Mandatory=$false)]
    [string]$JwtSecretKey = "YourSuperSecretKeyHere_MinimumLength32Characters_ChangeThisInProduction!",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild = $false
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Azure Deployment Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Azure CLI is installed
Write-Host "Checking Azure CLI installation..." -ForegroundColor Yellow
try {
    az version | Out-Null
    Write-Host "✓ Azure CLI is installed" -ForegroundColor Green
} catch {
    Write-Host "✗ Azure CLI is not installed. Please install it from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli" -ForegroundColor Red
    exit 1
}

# Login to Azure
Write-Host ""
Write-Host "Logging in to Azure..." -ForegroundColor Yellow
az account show 2>$null
if ($LASTEXITCODE -ne 0) {
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Failed to login to Azure" -ForegroundColor Red
        exit 1
    }
}
Write-Host "✓ Logged in to Azure" -ForegroundColor Green

# Create Resource Group
Write-Host ""
Write-Host "Creating Resource Group: $ResourceGroupName..." -ForegroundColor Yellow
az group create --name $ResourceGroupName --location $Location --output none
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Resource Group created/verified" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to create Resource Group" -ForegroundColor Red
    exit 1
}

# Create App Service Plan
Write-Host ""
Write-Host "Creating App Service Plan: $AppServicePlanName..." -ForegroundColor Yellow
az appservice plan create `
    --name $AppServicePlanName `
    --resource-group $ResourceGroupName `
    --sku B1 `
    --is-linux `
    --output none
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ App Service Plan created/verified" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to create App Service Plan" -ForegroundColor Red
    exit 1
}

# Create Web App
Write-Host ""
Write-Host "Creating Web App: $WebAppName..." -ForegroundColor Yellow
az webapp create `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --plan $AppServicePlanName `
    --runtime "DOTNET:8.0" `
    --output none
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Web App created/verified" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to create Web App" -ForegroundColor Red
    exit 1
}

# Enable WebSockets for SignalR
Write-Host ""
Write-Host "Enabling WebSockets..." -ForegroundColor Yellow
az webapp config set `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --web-sockets-enabled true `
    --output none
Write-Host "✓ WebSockets enabled" -ForegroundColor Green

# Create SQL Server
Write-Host ""
Write-Host "Creating SQL Server: $SqlServerName..." -ForegroundColor Yellow
az sql server create `
    --name $SqlServerName `
    --resource-group $ResourceGroupName `
    --location $Location `
    --admin-user $SqlAdminUser `
    --admin-password $SqlAdminPassword `
    --output none
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ SQL Server created/verified" -ForegroundColor Green
} else {
    Write-Host "! SQL Server may already exist or there was an error" -ForegroundColor Yellow
}

# Configure SQL Server Firewall
Write-Host ""
Write-Host "Configuring SQL Server Firewall..." -ForegroundColor Yellow
az sql server firewall-rule create `
    --resource-group $ResourceGroupName `
    --server $SqlServerName `
    --name AllowAzureServices `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0 `
    --output none
Write-Host "✓ Firewall configured" -ForegroundColor Green

# Create Database
Write-Host ""
Write-Host "Creating Database: $DatabaseName..." -ForegroundColor Yellow
az sql db create `
    --name $DatabaseName `
    --server $SqlServerName `
    --resource-group $ResourceGroupName `
    --service-objective S0 `
    --output none
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Database created/verified" -ForegroundColor Green
} else {
    Write-Host "! Database may already exist or there was an error" -ForegroundColor Yellow
}

# Configure Connection String
Write-Host ""
Write-Host "Configuring Connection String..." -ForegroundColor Yellow
$connectionString = "Server=tcp:$SqlServerName.database.windows.net,1433;Initial Catalog=$DatabaseName;Persist Security Info=False;User ID=$SqlAdminUser;Password=$SqlAdminPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
az webapp config connection-string set `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --connection-string-type SQLAzure `
    --settings DefaultConnection=$connectionString `
    --output none
Write-Host "✓ Connection String configured" -ForegroundColor Green

# Configure App Settings
Write-Host ""
Write-Host "Configuring App Settings..." -ForegroundColor Yellow
az webapp config appsettings set `
    --name $WebAppName `
    --resource-group $ResourceGroupName `
    --settings `
        "JwtSettings__SecretKey=$JwtSecretKey" `
        "JwtSettings__Issuer=ChatApp.Backend" `
        "JwtSettings__Audience=ChatApp.Client" `
        "ASPNETCORE_ENVIRONMENT=Production" `
    --output none
Write-Host "✓ App Settings configured" -ForegroundColor Green

# Build and Deploy
if (-not $SkipBuild) {
    Write-Host ""
    Write-Host "Building application..." -ForegroundColor Yellow
    dotnet publish -c Release -o ./publish
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Build failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Build completed" -ForegroundColor Green

    Write-Host ""
    Write-Host "Creating deployment package..." -ForegroundColor Yellow
    if (Test-Path "./app.zip") {
        Remove-Item "./app.zip" -Force
    }
    Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force
    Write-Host "✓ Package created" -ForegroundColor Green

    Write-Host ""
    Write-Host "Deploying to Azure..." -ForegroundColor Yellow
    az webapp deployment source config-zip `
        --name $WebAppName `
        --resource-group $ResourceGroupName `
        --src ./app.zip
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Deployment completed" -ForegroundColor Green
    } else {
        Write-Host "✗ Deployment failed" -ForegroundColor Red
        exit 1
    }

    # Cleanup
    Write-Host ""
    Write-Host "Cleaning up..." -ForegroundColor Yellow
    Remove-Item -Path ./publish -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item -Path ./app.zip -Force -ErrorAction SilentlyContinue
    Write-Host "✓ Cleanup completed" -ForegroundColor Green
}

# Display URLs
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Application URL: https://$WebAppName.azurewebsites.net" -ForegroundColor Yellow
Write-Host "Swagger URL: https://$WebAppName.azurewebsites.net/swagger" -ForegroundColor Yellow
Write-Host ""
Write-Host "SignalR Hubs:" -ForegroundColor Yellow
Write-Host "  Chat: https://$WebAppName.azurewebsites.net/hubs/chat" -ForegroundColor White
Write-Host "  Notifications: https://$WebAppName.azurewebsites.net/hubs/notification" -ForegroundColor White
Write-Host "  Posts: https://$WebAppName.azurewebsites.net/hubs/post" -ForegroundColor White
Write-Host ""
Write-Host "SQL Server: $SqlServerName.database.windows.net" -ForegroundColor Yellow
Write-Host "Database: $DatabaseName" -ForegroundColor Yellow
Write-Host ""
Write-Host "⚠️  IMPORTANT: Run database migrations!" -ForegroundColor Red
Write-Host "Use: dotnet ef database update --connection 'your-connection-string'" -ForegroundColor White
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Run database migrations" -ForegroundColor White
Write-Host "2. Update CORS settings in Program.cs with your Azure URL" -ForegroundColor White
Write-Host "3. Test the application at the URLs above" -ForegroundColor White
Write-Host "4. Configure custom domain (optional)" -ForegroundColor White
Write-Host "5. Set up SSL certificate (optional)" -ForegroundColor White
Write-Host ""


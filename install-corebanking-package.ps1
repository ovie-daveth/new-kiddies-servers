# CoreBankingService NuGet Package Installation Script
# Run this script after updating nuget.config with your custom feed URL

param(
    [Parameter(Mandatory=$false)]
    [string]$FeedUrl = "",
    
    [Parameter(Mandatory=$false)]
    [string]$Username = "",
    
    [Parameter(Mandatory=$false)]
    [string]$Password = "",
    
    [Parameter(Mandatory=$false)]
    [string]$PackageVersion = ""
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CoreBankingService Package Installer" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Step 1: Add NuGet source if provided
if ($FeedUrl) {
    Write-Host "Adding custom NuGet source..." -ForegroundColor Yellow
    
    if ($Username -and $Password) {
        dotnet nuget add source $FeedUrl --name "PremiumTrust" --username $Username --password $Password --store-password-in-clear-text
    } else {
        dotnet nuget add source $FeedUrl --name "PremiumTrust"
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ NuGet source added successfully`n" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed to add NuGet source`n" -ForegroundColor Red
    }
}

# Step 2: List available sources
Write-Host "Available NuGet sources:" -ForegroundColor Yellow
dotnet nuget list source
Write-Host ""

# Step 3: Clear NuGet cache (optional but recommended)
Write-Host "Clearing NuGet cache..." -ForegroundColor Yellow
dotnet nuget locals all --clear
Write-Host "✓ Cache cleared`n" -ForegroundColor Green

# Step 4: Install CoreBankingService package
Write-Host "Installing CoreBankingService package..." -ForegroundColor Yellow

if ($PackageVersion) {
    Write-Host "Installing version: $PackageVersion" -ForegroundColor Cyan
    dotnet add package CoreBankingService --version $PackageVersion
} else {
    Write-Host "Installing latest version" -ForegroundColor Cyan
    dotnet add package CoreBankingService
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ CoreBankingService package installed successfully`n" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to install package`n" -ForegroundColor Red
    Write-Host "Please check:" -ForegroundColor Yellow
    Write-Host "  1. Your NuGet feed URL is correct" -ForegroundColor White
    Write-Host "  2. You have access to the feed" -ForegroundColor White
    Write-Host "  3. The package name is correct (CoreBankingService)" -ForegroundColor White
    Write-Host "  4. Authentication credentials if required`n" -ForegroundColor White
    exit 1
}

# Step 5: Restore packages
Write-Host "Restoring all packages..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Packages restored successfully`n" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to restore packages`n" -ForegroundColor Red
    exit 1
}

# Step 6: Build project to verify
Write-Host "Building project to verify installation..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✓ Build successful!`n" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Installation Complete!" -ForegroundColor Green
    Write-Host "========================================`n" -ForegroundColor Cyan
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host "  1. Update Program.cs to register CoreBanking services" -ForegroundColor White
    Write-Host "  2. Create EmailService using ICommunicationService" -ForegroundColor White
    Write-Host "  3. See COREBANKING_NUGET_SETUP.md for details`n" -ForegroundColor White
} else {
    Write-Host "`n✗ Build failed`n" -ForegroundColor Red
    Write-Host "Please check for compilation errors" -ForegroundColor Yellow
    exit 1
}


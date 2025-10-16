using ChatApp.Backend.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ChatApp.Backend.Services;

public class CoreBankingService : ICoreBankingService
{
    private readonly CoreBankingSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoreBankingService> _logger;

    public CoreBankingService(
        IOptions<CoreBankingSettings> settings,
        HttpClient httpClient,
        ILogger<CoreBankingService> logger)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
        _logger = logger;

        // Validate settings
        if (!_settings.IsValid())
        {
            throw new InvalidOperationException("CoreBanking settings are not properly configured");
        }

        // Configure HttpClient
        _httpClient.BaseAddress = new Uri(_settings.GetEffectiveBaseUrl());
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("X-Client-Id", _settings.ClientId);
    }

    public async Task<BankAccountDto> CreateAccount(int userId, string accountType)
    {
        try
        {
            var endpoint = _settings.AccountCreationEndpoint;
            
            var requestData = new
            {
                userId,
                accountType,
                currency = _settings.DefaultCurrency,
                merchantId = _settings.MerchantId
            };

            if (_settings.EnableLogging)
            {
                _logger.LogInformation("Creating bank account for user {UserId}, Type: {AccountType}", 
                    userId, accountType);
            }

            var response = await PostAsync<BankAccountDto>(endpoint, requestData);
            
            if (_settings.EnableLogging)
            {
                _logger.LogInformation("Bank account created successfully. Account: {AccountNumber}", 
                    response?.AccountNumber);
            }

            return response ?? throw new Exception("Failed to create account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bank account for user {UserId}", userId);
            throw;
        }
    }

    public async Task<decimal> GetAccountBalance(string accountNumber)
    {
        try
        {
            var endpoint = $"{_settings.BalanceInquiryEndpoint}/{accountNumber}";
            
            if (_settings.EnableLogging)
            {
                _logger.LogInformation("Fetching balance for account {AccountNumber}", accountNumber);
            }

            var response = await GetAsync<BalanceResponse>(endpoint);
            
            return response?.Balance ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching balance for account {AccountNumber}", accountNumber);
            throw;
        }
    }

    public async Task<TransactionResultDto> Transfer(
        string fromAccount, 
        string toAccount, 
        decimal amount, 
        string narration)
    {
        try
        {
            var endpoint = _settings.TransferEndpoint;
            
            var requestData = new
            {
                fromAccount,
                toAccount,
                amount,
                narration,
                currency = _settings.DefaultCurrency,
                merchantId = _settings.MerchantId
            };

            if (_settings.EnableLogging)
            {
                _logger.LogInformation(
                    "Initiating transfer: {Amount} {Currency} from {From} to {To}", 
                    amount, _settings.DefaultCurrency, fromAccount, toAccount);
            }

            var response = await PostAsync<TransactionResultDto>(endpoint, requestData);
            
            if (_settings.EnableLogging)
            {
                _logger.LogInformation(
                    "Transfer completed. Reference: {Reference}, Success: {Success}", 
                    response?.TransactionReference, response?.Success);
            }

            return response ?? throw new Exception("Transfer failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transfer");
            throw;
        }
    }

    public async Task<List<TransactionDto>> GetTransactionHistory(
        string accountNumber, 
        int skip = 0, 
        int take = 20)
    {
        try
        {
            var endpoint = $"{_settings.TransactionEndpoint}?accountNumber={accountNumber}&skip={skip}&take={take}";
            
            if (_settings.EnableLogging)
            {
                _logger.LogInformation("Fetching transaction history for account {AccountNumber}", accountNumber);
            }

            var response = await GetAsync<List<TransactionDto>>(endpoint);
            
            return response ?? new List<TransactionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching transaction history for account {AccountNumber}", accountNumber);
            throw;
        }
    }

    public async Task<BankAccountValidationDto> ValidateAccount(string accountNumber, string bankCode)
    {
        try
        {
            var endpoint = $"/api/accounts/validate?accountNumber={accountNumber}&bankCode={bankCode}";
            
            if (_settings.EnableLogging)
            {
                _logger.LogInformation("Validating account {AccountNumber} at bank {BankCode}", 
                    accountNumber, bankCode);
            }

            var response = await GetAsync<BankAccountValidationDto>(endpoint);
            
            return response ?? new BankAccountValidationDto { IsValid = false };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating account {AccountNumber}", accountNumber);
            return new BankAccountValidationDto { IsValid = false };
        }
    }

    // Helper methods
    private async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
    }

    private async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
    }

    private class BalanceResponse
    {
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}


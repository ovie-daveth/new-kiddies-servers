namespace ChatApp.Backend.Configuration;

/// <summary>
/// Core Banking configuration settings
/// </summary>
public class CoreBankingSettings
{
    /// <summary>
    /// Core Banking API Base URL
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// API Key for authentication
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// API Secret for authentication
    /// </summary>
    public string ApiSecret { get; set; } = string.Empty;

    /// <summary>
    /// Client ID for OAuth/API authentication
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client Secret for OAuth/API authentication
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Merchant ID or Institution ID
    /// </summary>
    public string MerchantId { get; set; } = string.Empty;

    /// <summary>
    /// API timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Enable retry on failure
    /// </summary>
    public bool EnableRetry { get; set; } = true;

    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Enable logging of requests/responses
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Environment (Development, Staging, Production)
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Account creation endpoint
    /// </summary>
    public string AccountCreationEndpoint { get; set; } = "/api/accounts/create";

    /// <summary>
    /// Balance inquiry endpoint
    /// </summary>
    public string BalanceInquiryEndpoint { get; set; } = "/api/accounts/balance";

    /// <summary>
    /// Transaction endpoint
    /// </summary>
    public string TransactionEndpoint { get; set; } = "/api/transactions";

    /// <summary>
    /// Transfer endpoint
    /// </summary>
    public string TransferEndpoint { get; set; } = "/api/transfers";

    /// <summary>
    /// Webhook callback URL for notifications
    /// </summary>
    public string WebhookCallbackUrl { get; set; } = string.Empty;

    /// <summary>
    /// Webhook secret for signature validation
    /// </summary>
    public string WebhookSecret { get; set; } = string.Empty;

    /// <summary>
    /// Enable sandbox mode for testing
    /// </summary>
    public bool UseSandbox { get; set; } = false;

    /// <summary>
    /// Sandbox API Base URL
    /// </summary>
    public string SandboxBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Default currency code (e.g., NGN, USD)
    /// </summary>
    public string DefaultCurrency { get; set; } = "NGN";

    /// <summary>
    /// Bank code or routing number
    /// </summary>
    public string BankCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets the effective base URL based on sandbox mode
    /// </summary>
    public string GetEffectiveBaseUrl()
    {
        return UseSandbox && !string.IsNullOrEmpty(SandboxBaseUrl) 
            ? SandboxBaseUrl 
            : BaseUrl;
    }

    /// <summary>
    /// Validates that required settings are configured
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(BaseUrl) &&
               !string.IsNullOrEmpty(ApiKey) &&
               !string.IsNullOrEmpty(ClientId);
    }
}


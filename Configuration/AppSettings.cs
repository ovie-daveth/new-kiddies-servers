using System.Text.Json;

namespace ChatApp.Backend.Configuration;

public class AppSettings
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AppSettings> _logger;
    private readonly Dictionary<string, string> _cache = new();
    private readonly string _parameterStoreBaseUrl;
    private readonly string _apiKey;

    // Parameter Store keys from appsettings.json
    public string CoreBankingParameterName { get; set; } = string.Empty;
    public string OnboardingCorrespondence { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;

    public AppSettings(IConfiguration configuration, ILogger<AppSettings> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        
        // Load parameter names from appsettings.json
        var appSettingsSection = configuration.GetSection("AppSettings");
        CoreBankingParameterName = appSettingsSection["CoreBankingParameterName"] ?? "";
        OnboardingCorrespondence = appSettingsSection["OnboardingCorrespondence"] ?? "";
        FromEmail = appSettingsSection["FromEmail"] ?? "";
        
        // Load Parameter Store connection details
        _parameterStoreBaseUrl = configuration["ParameterStore:BaseUrl"] ?? "";
        _apiKey = configuration["ParameterStore:ApiKey"] ?? "";
        
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        }
    }

    /// <summary>
    /// Get settings from Parameter Store as strongly-typed object
    /// </summary>
    public async Task<T> GetSettingsAsync<T>(string parameterName) where T : class, new()
    {
        try
        {
            // Check cache first
            if (_cache.ContainsKey(parameterName))
            {
                return JsonSerializer.Deserialize<T>(_cache[parameterName]) ?? new T();
            }

            _logger.LogInformation("Fetching parameter from Parameter Store: {ParameterName}", parameterName);

            // Call your company's Parameter Store API
            var url = $"{_parameterStoreBaseUrl}/api/parameters/{parameterName}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var value = await response.Content.ReadAsStringAsync();

            // Cache the value
            _cache[parameterName] = value;

            _logger.LogInformation("Successfully fetched parameter: {ParameterName}", parameterName);

            return JsonSerializer.Deserialize<T>(value) ?? new T();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching parameter {ParameterName} from Parameter Store", parameterName);
            throw;
        }
    }

    /// <summary>
    /// Get settings from Parameter Store as string
    /// </summary>
    public async Task<string> GetSettingsAsync(string parameterName)
    {
        try
        {
            // Check cache first
            if (_cache.ContainsKey(parameterName))
            {
                return _cache[parameterName];
            }

            _logger.LogInformation("Fetching parameter from Parameter Store: {ParameterName}", parameterName);

            // Call your company's Parameter Store API
            var url = $"{_parameterStoreBaseUrl}/api/parameters/{parameterName}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var value = await response.Content.ReadAsStringAsync();

            // Cache the value
            _cache[parameterName] = value;

            _logger.LogInformation("Successfully fetched parameter: {ParameterName}", parameterName);

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching parameter {ParameterName} from Parameter Store", parameterName);
            throw;
        }
    }

    /// <summary>
    /// Clear cached parameters (useful for testing or refreshing config)
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
        _logger.LogInformation("Parameter Store cache cleared");
    }

    /// <summary>
    /// Get CoreBanking configuration from Parameter Store
    /// </summary>
    public async Task<CoreBankingParameterStore> GetCoreBankingSettings()
    {
        return await GetSettingsAsync<CoreBankingParameterStore>(CoreBankingParameterName);
    }
}


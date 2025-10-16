namespace ChatApp.Backend.Configuration;

/// <summary>
/// Root configuration from Parameter Store: AppSettings/CoreBanking
/// </summary>
public class CoreBankingParameterStore
{
    public CoreEnquiry CoreEnquiry { get; set; } = new();
    public CoreParameters CoreParameters { get; set; } = new();
    public BvnParameter BvnParameter { get; set; } = new();
    public AccountOpeningParameter AccountOpeningParameter { get; set; } = new();
    public Authentication Authentication { get; set; } = new();
    public Communications Communications { get; set; } = new();
    public NipParameter NipParameter { get; set; } = new();
    public Externals Externals { get; set; } = new();
    public Mandate Mandate { get; set; } = new();
    public NEFT NEFT { get; set; } = new();
    public VirtualAccountParameter VirtualAccountParameter { get; set; } = new();
    public SMSService SMSService { get; set; } = new();
    public AccountSegmentation AccountSegmentation { get; set; } = new();
    public AccountStatementWithStampSetting AccountStatementWithStampSetting { get; set; } = new();
    public VirtualCardParameter VirtualCardParameter { get; set; } = new();
    public VisaCardIssuanceParameter VisaCardIssuanceParameter { get; set; } = new();
}

public class CoreEnquiry
{
    public bool IsEnabled { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public class CoreParameters
{
    public bool IsEnabled { get; set; }
    public string AppToken { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}

public class BvnParameter
{
    public string BaseUrl { get; set; } = string.Empty;
    public string VerifySingleBVn { get; set; } = string.Empty;
    public string GetSingleBvn { get; set; } = string.Empty;
    public JWTSettings JWT { get; set; } = new();
}

public class JWTSettings
{
    public string ValidAudience { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
}

public class AccountOpeningParameter
{
    public string BaseUrl { get; set; } = string.Empty;
    public string GetAccountLocations { get; set; } = string.Empty;
    public string GetAccountLocation { get; set; } = string.Empty;
    public string GetAccountDocument { get; set; } = string.Empty;
    public string GetCustomerCategories { get; set; } = string.Empty;
    public string GetCustomerClass { get; set; } = string.Empty;
    public string GetCustomerCountries { get; set; } = string.Empty;
    public string ValidateTin { get; set; } = string.Empty;
}

public class Authentication
{
    public bool IsEnabled { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string ValidateCredentials { get; set; } = string.Empty;
    public string UserDetails { get; set; } = string.Empty;
}

public class Communications
{
    public string BaseUrl { get; set; } = string.Empty;
    public string SendSMS { get; set; } = string.Empty;
    public string SendEmail { get; set; } = string.Empty;
    public string SendMailWithAttachment { get; set; } = string.Empty;
}

public class NipParameter
{
    public string ServiceUrl { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string TestBankCode { get; set; } = string.Empty;
    public string TestAccount { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
    public string ChannelCode { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public bool NipEnabled { get; set; }
}

public class Externals
{
    public string BaseUrl { get; set; } = string.Empty;
    public string GetBanks { get; set; } = string.Empty;
}

public class Mandate
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Authorization { get; set; } = string.Empty;
}

public class NEFT
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class VirtualAccountParameter
{
    public VirtualTransferCredentials VirtualTransferCredentials { get; set; } = new();
    public VirtualAccountCredentials VirtualAccountCredentials { get; set; } = new();
}

public class VirtualTransferCredentials
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ClientIv { get; set; } = string.Empty;
}

public class VirtualAccountCredentials
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ClientIv { get; set; } = string.Empty;
    public List<VirtualAccountMerchantCredentials> VirtualAccountMerchantCredentials { get; set; } = new();
}

public class VirtualAccountMerchantCredentials
{
    public string MerchantName { get; set; } = string.Empty;
    public string MerchantClientId { get; set; } = string.Empty;
    public string AccountPrefix { get; set; } = string.Empty;
}

public class SMSService
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public class AccountSegmentation
{
    public string Account { get; set; } = string.Empty;
    public string Business { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
}

public class AccountStatementWithStampSetting
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientKey { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class VirtualCardParameter
{
    public VirtualCardCredentials VirtualCardCredentials { get; set; } = new();
}

public class VirtualCardCredentials
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ClientIv { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool IsDev { get; set; }
}

public class VisaCardIssuanceParameter
{
    public VisaCardIssuanceCredentials VisaCardIssuanceCredentials { get; set; } = new();
}

public class VisaCardIssuanceCredentials
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ClientIv { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool IsDev { get; set; }
}


namespace ChatApp.Backend.Services;

/// <summary>
/// Interface for Core Banking operations
/// </summary>
public interface ICoreBankingService
{
    /// <summary>
    /// Create a new bank account for a user
    /// </summary>
    Task<BankAccountDto> CreateAccount(int userId, string accountType);

    /// <summary>
    /// Get account balance
    /// </summary>
    Task<decimal> GetAccountBalance(string accountNumber);

    /// <summary>
    /// Perform a fund transfer
    /// </summary>
    Task<TransactionResultDto> Transfer(string fromAccount, string toAccount, decimal amount, string narration);

    /// <summary>
    /// Get transaction history
    /// </summary>
    Task<List<TransactionDto>> GetTransactionHistory(string accountNumber, int skip = 0, int take = 20);

    /// <summary>
    /// Validate bank account
    /// </summary>
    Task<BankAccountValidationDto> ValidateAccount(string accountNumber, string bankCode);
}

public class BankAccountDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TransactionResultDto
{
    public bool Success { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
}

public class TransactionDto
{
    public string Reference { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class BankAccountValidationDto
{
    public bool IsValid { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}


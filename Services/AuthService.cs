using ChatApp.Backend.Data;
using ChatApp.Backend.DTOs;
using ChatApp.Backend.Models;
using ChatApp.Backend.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Backend.Services;

public class AuthService : IAuthService
{
    private readonly ChatDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly AppSettings _appSettings;
    private readonly ILogger<AuthService> _logger;
    private string? _fromEmail;
    private CoreBankingParameterStore? _coreBankingSettings;

    public AuthService(
        ChatDbContext context, 
        IConfiguration configuration, 
        AppSettings appSettings,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _appSettings = appSettings;
        _logger = logger;

        // Test Parameter Store connection on startup
        InitializeParameterStoreAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeParameterStoreAsync()
    {
        try
        {
            _logger.LogInformation("🔌 Testing Parameter Store connection...");
            
            // Fetch CoreBanking settings from Parameter Store
            _coreBankingSettings = await _appSettings.GetCoreBankingSettings();
            
            _logger.LogInformation("✅ Parameter Store connected successfully!");
            _logger.LogInformation("📡 Communications API: {BaseUrl}", _coreBankingSettings.Communications.BaseUrl);
            _logger.LogInformation("📱 SMS Service API: {BaseUrl}", _coreBankingSettings.SMSService.BaseUrl);
            _logger.LogInformation("🏦 Core Enquiry API: {BaseUrl}", _coreBankingSettings.CoreEnquiry.BaseUrl);
            
            // Fetch FromEmail setting
            _fromEmail = await _appSettings.GetSettingsAsync(_appSettings.FromEmail);
            _logger.LogInformation("📧 From Email configured: {FromEmail}", _fromEmail);
            
            _logger.LogInformation("🎉 CoreBanking services initialized successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to connect to Parameter Store!");
            _logger.LogWarning("⚠️  Authentication service will continue, but email features may not work");
            // Don't throw - allow service to start even if Parameter Store is unavailable
        }
    }

    public async Task<AuthResponseDto> Register(RegisterDto registerDto)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            throw new Exception("User with this email already exists");
        }

        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
        {
            throw new Exception("Username is already taken");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            DisplayName = registerDto.DisplayName ?? registerDto.Username,
            IsOnline = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token
        };
    }

    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new Exception("Invalid username or password");
        }

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token
        };
    }

    public async Task<UserDto?> GetUserById(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsOnline = user.IsOnline,
            LastSeen = user.LastSeen
        };
    }

    public async Task<List<UserDto>> SearchUsers(string query)
    {
        var users = await _context.Users
            .Where(u => u.Username.Contains(query) || (u.DisplayName != null && u.DisplayName.Contains(query)))
            .Take(20)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName,
                ProfilePictureUrl = u.ProfilePictureUrl,
                IsOnline = u.IsOnline,
                LastSeen = u.LastSeen
            })
            .ToListAsync();

        return users;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


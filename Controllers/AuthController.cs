using ChatApp.Backend.DTOs;
using ChatApp.Backend.Services;
using ChatApp.Backend.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AppSettings _appSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        AppSettings appSettings,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _appSettings = appSettings;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var response = await _authService.Register(registerDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.Login(loginDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);
        var user = await _authService.GetUserById(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [Authorize]
    [HttpGet("users/search")]
    public async Task<ActionResult<List<UserDto>>> SearchUsers([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new { message = "Query parameter is required" });
        }

        var users = await _authService.SearchUsers(query);
        return Ok(users);
    }

    [Authorize]
    [HttpGet("users/{userId}")]
    public async Task<ActionResult<UserDto>> GetUser(int userId)
    {
        var user = await _authService.GetUserById(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Test endpoint to verify Parameter Store and CoreBanking connection
    /// </summary>
    [HttpGet("test-corebanking")]
    public async Task<IActionResult> TestCoreBankingConnection()
    {
        try
        {
            _logger.LogInformation("🧪 Testing CoreBanking connection...");

            // Fetch CoreBanking settings from Parameter Store
            var settings = await _appSettings.GetCoreBankingSettings();

            // Get FromEmail
            var fromEmail = await _appSettings.GetSettingsAsync(_appSettings.FromEmail);

            _logger.LogInformation("✅ CoreBanking connection test successful!");

            return Ok(new
            {
                success = true,
                message = "CoreBanking services connected successfully!",
                parameterStore = new
                {
                    connected = true,
                    parameterName = _appSettings.CoreBankingParameterName
                },
                coreBanking = new
                {
                    coreEnquiry = new
                    {
                        baseUrl = settings.CoreEnquiry.BaseUrl,
                        isEnabled = settings.CoreEnquiry.IsEnabled,
                        apiKeyLength = settings.CoreEnquiry.ApiKey?.Length ?? 0
                    },
                    communications = new
                    {
                        baseUrl = settings.Communications.BaseUrl,
                        endpoints = new
                        {
                            sendSMS = settings.Communications.SendSMS,
                            sendEmail = settings.Communications.SendEmail
                        }
                    },
                    smsService = new
                    {
                        baseUrl = settings.SMSService.BaseUrl,
                        apiKeyLength = settings.SMSService.ApiKey?.Length ?? 0
                    },
                    authentication = new
                    {
                        baseUrl = settings.Authentication.BaseUrl,
                        isEnabled = settings.Authentication.IsEnabled
                    }
                },
                email = new
                {
                    fromEmail = fromEmail
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ CoreBanking connection test failed!");

            return StatusCode(500, new
            {
                success = false,
                message = "Failed to connect to CoreBanking services",
                error = ex.Message,
                details = "Check your .env file and Parameter Store configuration"
            });
        }
    }
}


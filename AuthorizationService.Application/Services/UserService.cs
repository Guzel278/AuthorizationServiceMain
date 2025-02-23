
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthorizationService.Application.Interfaces;
using AuthorizationService.Domain.Models;
using AuthorizationService.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static System.Net.WebRequestMethods;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public UserService(ApplicationDbContext context, IConfiguration configuration, ITokenService tokenService)
    {
        _context = context;
        _configuration = configuration;
        _tokenService = tokenService;
    }


    public async Task<bool> RegisterUserAsync(User user, string password)
    {
        // Проверка существующего пользователя
        if (_context.Users.Any(u => u.Username == user.Username || u.Email == user.Email))
            return false;

        // Генерация хеша пароля 
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }

    public string GenerateJwtToken(User user) => _tokenService.GenerateJwtToken(user);

    public async Task<Role?> GetRoleByIdAsync(int roleId)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task SaveUserTokenAsync(int userId, string token)
    {
        var newToken = new Token
        {
            UserId = userId,
            TokenValue = token,
            Expiry = DateTime.UtcNow.AddHours(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tokens.Add(newToken);
        await _context.SaveChangesAsync();
    }

    public async Task LogUserActionAsync(int userId, string action)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            ActionTime = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RevokeUserTokenAsync(string token)
    {
        var existingToken = await _context.Tokens
            .FirstOrDefaultAsync(t => t.TokenValue == token && !t.IsRevoked);

        if (existingToken == null)
            return false;

        existingToken.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerateOtpAsync(int userId)
    {
        var otpCode = new Random().Next(100000, 999999).ToString();  // Генерация 6-значного кода
        var otp = new OTP
        {
            Code = otpCode,
            Expiry = DateTime.UtcNow.AddMinutes(5), // Код действует 5 минут
            UserId = userId,
            IsUsed = false
        };

        _context.OTPs.Add(otp);
        await _context.SaveChangesAsync();

        return otpCode;
    }
}

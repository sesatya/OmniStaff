using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OmniStaff.Application.Services;

/// <summary>
/// Implements US-001 (User Login). Replace the in-memory lookup with a real
/// repository call once OmniStaff. Infrastructure exposes one.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepo;
    private const int MaxFailedAttempts = 5;

    public AuthService(IConfiguration configuration, IUserRepository userRepo)
    {
        _configuration = configuration;
        _userRepo = userRepo;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _userRepo.GetByEmailAsync(email);
        if (user is null) return null;

        // verify password
        if (!PasswordHasher.VerifyPassword(password, user.PasswordHash)) return null;

        var jwtSection = _configuration.GetSection("Jwt");
        var expiryValue = jwtSection["ExpiryMinutes"];
        var expiryMinutes = int.TryParse(expiryValue, out var parsed) ? parsed : 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse(tokenString, expiresAt, user.DisplayName);
    }
}

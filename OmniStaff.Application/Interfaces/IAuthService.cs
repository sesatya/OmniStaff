using OmniStaff.Application.Dtos;

namespace OmniStaff.Application.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Validates credentials and issues a JWT. Returns null on invalid credentials (US-001 AC-2).
    /// </summary>
    Task<LoginResponse?> LoginAsync(string email, string password);
}

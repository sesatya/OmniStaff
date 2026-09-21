namespace OmniStaff. Application.Dtos;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, DateTime ExpiresAtUtc, string DisplayName);

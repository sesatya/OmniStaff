using System;

namespace OmniStaff.Application.Dtos;

public record UserDto(Guid Id, string Email, string DisplayName, DateTime CreatedAtUtc);

public record CreateUserDto(string Email, string DisplayName, string PasswordHash);

public record UpdateUserDto(string? Email, string? DisplayName, string? PasswordHash);

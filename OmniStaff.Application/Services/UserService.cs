using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;
using OmniStaff.Domain.Entities;
using OmniStaff.Application.Services;
using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;
using OmniStaff.Domain.Entities;

namespace OmniStaff.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(u => new UserDto(u.Id, u.Email, u.DisplayName, u.CreatedAtUtc));
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var u = await _repo.GetByIdAsync(id);
        if (u is null) return null;
        return new UserDto(u.Id, u.Email, u.DisplayName, u.CreatedAtUtc);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            PasswordHash = PasswordHasher.HashPassword(dto.PasswordHash),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repo.CreateAsync(user);
        return new UserDto(user.Id, user.Email, user.DisplayName, user.CreatedAtUtc);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user is null) return false;

        if (!string.IsNullOrWhiteSpace(dto.Email)) user.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.DisplayName)) user.DisplayName = dto.DisplayName;
        if (!string.IsNullOrWhiteSpace(dto.PasswordHash)) user.PasswordHash = PasswordHasher.HashPassword(dto.PasswordHash);

        await _repo.UpdateAsync(user);
        return true;
    }



    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user is null) return false;
        await _repo.DeleteAsync(user);
        return true;
    }
}

using OmniStaff.Domain.Entities;

namespace OmniStaff.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}

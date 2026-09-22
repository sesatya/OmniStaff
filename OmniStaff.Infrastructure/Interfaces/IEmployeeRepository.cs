using OmniStaff.Domain.Entities;

namespace OmniStaff.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(Guid id);
    Task CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
}

using OmniStaff.Application.Dtos;

namespace OmniStaff.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateEmployeeDto dto);
    Task<bool> DeleteAsync(Guid id);
}

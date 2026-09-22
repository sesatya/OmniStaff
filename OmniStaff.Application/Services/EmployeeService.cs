using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;
using OmniStaff.Domain.Entities;

namespace OmniStaff.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(e => new EmployeeDto(e.Id, e.UserId, e.EmployeeNumber, e.FirstName, e.LastName, e.ManagerId, e.HireDate, e.Department, e.CreatedAtUtc));
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e is null) return null;
        return new EmployeeDto(e.Id, e.UserId, e.EmployeeNumber, e.FirstName, e.LastName, e.ManagerId, e.HireDate, e.Department, e.CreatedAtUtc);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var entity = new Employee
        {
            UserId = dto.UserId,
            EmployeeNumber = dto.EmployeeNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ManagerId = dto.ManagerId,
            HireDate = dto.HireDate,
            Department = dto.Department,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repo.CreateAsync(entity);
        return new EmployeeDto(entity.Id, entity.UserId, entity.EmployeeNumber, entity.FirstName, entity.LastName, entity.ManagerId, entity.HireDate, entity.Department, entity.CreatedAtUtc);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;

        if (!string.IsNullOrWhiteSpace(dto.EmployeeNumber)) existing.EmployeeNumber = dto.EmployeeNumber;
        if (!string.IsNullOrWhiteSpace(dto.FirstName)) existing.FirstName = dto.FirstName;
        if (!string.IsNullOrWhiteSpace(dto.LastName)) existing.LastName = dto.LastName;
        if (dto.ManagerId.HasValue) existing.ManagerId = dto.ManagerId;
        existing.HireDate = dto.HireDate;
        if (!string.IsNullOrWhiteSpace(dto.Department)) existing.Department = dto.Department;

        await _repo.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;
        await _repo.DeleteAsync(existing);
        return true;
    }
}

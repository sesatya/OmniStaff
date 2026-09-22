using Microsoft.EntityFrameworkCore;
using OmniStaff.Application.Interfaces;
using OmniStaff.Domain.Entities;
using OmniStaff.Infrastructure.Persistence;

namespace OmniStaff.Infrastructure.Services;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _db.Employees.AsNoTracking().ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _db.Employees.FindAsync(id);
    }

    public async Task CreateAsync(Employee employee)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        _db.Employees.Update(employee);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();
    }
}

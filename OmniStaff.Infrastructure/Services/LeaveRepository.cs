using Microsoft.EntityFrameworkCore;
using OmniStaff.Application.Interfaces;
using OmniStaff.Domain.Entities;
using OmniStaff.Infrastructure.Persistence;

namespace OmniStaff.Infrastructure.Services;

public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext _db;

    public LeaveRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task CreateAsync(LeaveRequest request)
    {
        _db.LeaveRequests.Add(request);
        await _db.SaveChangesAsync();
    }

    public async Task<LeaveRequest?> GetByIdAsync(Guid id)
    {
        return await _db.LeaveRequests.FindAsync(id);
    }

    public async Task UpdateAsync(LeaveRequest request)
    {
        _db.LeaveRequests.Update(request);
        await _db.SaveChangesAsync();
    }
}

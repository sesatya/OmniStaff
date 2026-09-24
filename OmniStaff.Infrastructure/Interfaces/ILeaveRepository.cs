using OmniStaff.Domain.Entities;

namespace OmniStaff.Application.Interfaces;

public interface ILeaveRepository
{
    Task CreateAsync(LeaveRequest request);
    Task<LeaveRequest?> GetByIdAsync(Guid id);
    Task UpdateAsync(LeaveRequest request);
    Task AddApprovalAsync(LeaveRequestApproval approval);
    Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(Guid employeeId);
    Task<IEnumerable<LeaveRequest>> GetByManagerAsync(Guid managerId);
}

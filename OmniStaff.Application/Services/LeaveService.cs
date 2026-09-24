using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;
using OmniStaff.Domain.Entities;

namespace OmniStaff.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IEmailService _emailService;

    public LeaveService(ILeaveRepository leaveRepo, IEmployeeRepository employeeRepo, IEmailService emailService)
    {
        _leaveRepo = leaveRepo;
        _employeeRepo = employeeRepo;
        _emailService = emailService;
    }

    public async Task<LeaveRequestDto> ApplyAsync(CreateLeaveRequestDto dto)
    {
        // determine manager
        var employee = await _employeeRepo.GetByIdAsync(dto.EmployeeId);
        if (employee is null) throw new ArgumentException("Employee not found", nameof(dto.EmployeeId));

        var managerId = employee.ManagerId;

        var request = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalDays = dto.TotalDays,
            Status = LeaveRequestStatus.Pending,
            ManagerId = managerId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _leaveRepo.CreateAsync(request);

        // notify manager via email
        if (managerId.HasValue)
        {
            var manager = await _employeeRepo.GetByIdAsync(managerId.Value);
            if (manager is not null)
            {
                // manager's user email
                // Assuming Employee.User is not loaded here; fetch user info via IUserRepository if needed
                var userRepo = (IUserRepository?)null; // not available here
                // Simplified: send to manager email if User is available via Employee.User
                // For now, attempt to use manager.User?.Email
                var managerEmail = manager.User?.Email;
                if (!string.IsNullOrWhiteSpace(managerEmail))
                {
                    var subject = "Leave Approval Request";
                    var body = $"Employee {employee.FirstName} {employee.LastName} applied for leave from {dto.StartDate:d} to {dto.EndDate:d}.";
                    await _emailService.SendEmailAsync(managerEmail, subject, body);
                }
            }
        }

        return new LeaveRequestDto(request.Id, request.EmployeeId, request.LeaveTypeId, request.StartDate, request.EndDate, request.TotalDays, request.Status.ToString(), request.ManagerId, request.ManagerComment, request.CreatedAtUtc);
    }

    public async Task<bool> ApproveAsync(Guid requestId, ApproveLeaveDto dto)
    {
        var req = await _leaveRepo.GetByIdAsync(requestId);
        if (req is null) return false;

        // Only the assigned manager (if present) should approve/reject
        if (req.ManagerId.HasValue && req.ManagerId.Value != dto.ApproverEmployeeId)
        {
            // unauthorized -- silence as false
            return false;
        }

        req.Status = dto.Approve ? LeaveRequestStatus.Approved : LeaveRequestStatus.Rejected;
        req.ManagerComment = dto.Comment;
        req.ManagerId = dto.ApproverEmployeeId;
        req.UpdatedAtUtc = DateTime.UtcNow;

        var approval = new LeaveRequestApproval
        {
            LeaveRequestId = req.Id,
            ApproverEmployeeId = dto.ApproverEmployeeId,
            Action = req.Status,
            Comment = dto.Comment,
            ActionAt = DateTime.UtcNow
        };

        await _leaveRepo.AddApprovalAsync(approval);
        await _leaveRepo.UpdateAsync(req);

        // notify employee
        var employee = await _employeeRepo.GetByIdAsync(req.EmployeeId);
        var employeeEmail = employee?.User?.Email;
        if (!string.IsNullOrWhiteSpace(employeeEmail))
        {
            var subject = $"Your leave request has been {(dto.Approve ? "approved" : "rejected")}";
            var body = $"Your leave from {req.StartDate:d} to {req.EndDate:d} was {(dto.Approve ? "approved" : "rejected")}. Comment: {dto.Comment}";
            await _emailService.SendEmailAsync(employeeEmail, subject, body);
        }

        return true;
    }
}

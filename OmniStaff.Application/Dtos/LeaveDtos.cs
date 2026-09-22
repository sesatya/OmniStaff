using System;

namespace OmniStaff.Application.Dtos;

public record LeaveRequestDto(
    Guid Id,
    Guid EmployeeId,
    Guid LeaveTypeId,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalDays,
    string Status,
    Guid? ManagerId,
    string? ManagerComment,
    DateTime CreatedAtUtc
);

public record CreateLeaveRequestDto(
    Guid EmployeeId,
    Guid LeaveTypeId,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalDays
);

public record ApproveLeaveDto(
    Guid ApproverEmployeeId,
    bool Approve,
    string? Comment
);

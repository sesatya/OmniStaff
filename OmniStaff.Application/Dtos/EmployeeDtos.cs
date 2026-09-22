using System;

namespace OmniStaff.Application.Dtos;

public record EmployeeDto(
    Guid Id,
    Guid UserId,
    string? EmployeeNumber,
    string FirstName,
    string LastName,
    Guid? ManagerId,
    DateTime? HireDate,
    string? Department,
    DateTime CreatedAtUtc
);

public record CreateEmployeeDto(
    Guid UserId,
    string? EmployeeNumber,
    string FirstName,
    string LastName,
    Guid? ManagerId,
    DateTime? HireDate,
    string? Department
);

public record UpdateEmployeeDto(
    string? EmployeeNumber,
    string? FirstName,
    string? LastName,
    Guid? ManagerId,
    DateTime? HireDate,
    string? Department
);

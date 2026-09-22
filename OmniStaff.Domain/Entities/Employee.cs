namespace OmniStaff.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public string? EmployeeNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Department { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

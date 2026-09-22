namespace OmniStaff.Domain.Entities;

public class LeaveBalance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public int Year { get; set; }
    public decimal Entitlement { get; set; }
    public decimal Used { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

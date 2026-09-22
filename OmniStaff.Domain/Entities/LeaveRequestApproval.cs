namespace OmniStaff.Domain.Entities;

public class LeaveRequestApproval
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LeaveRequestId { get; set; }
    public Guid ApproverEmployeeId { get; set; }
    public DateTime ActionAt { get; set; } = DateTime.UtcNow;
    public LeaveRequestStatus Action { get; set; }
    public string? Comment { get; set; }
}

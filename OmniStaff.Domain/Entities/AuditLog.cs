namespace OmniStaff.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EntityName { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid? PerformedBy { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
}

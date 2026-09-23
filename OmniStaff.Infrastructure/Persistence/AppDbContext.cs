using Microsoft.EntityFrameworkCore;
using OmniStaff.Domain.Entities;

namespace OmniStaff.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveRequestApproval> LeaveRequestApprovals => Set<LeaveRequestApproval>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.DisplayName).IsRequired().HasMaxLength(128);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasOne(e => e.User).WithOne().HasForeignKey<Employee>(e => e.UserId);
            entity.HasOne(e => e.Manager).WithMany().HasForeignKey(e => e.ManagerId);
            entity.HasIndex(e => e.ManagerId);
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasIndex(lt => lt.Name).IsUnique();
            entity.Property(lt => lt.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.HasIndex(lb => new { lb.EmployeeId, lb.LeaveTypeId, lb.Year }).IsUnique();
            entity.HasOne< Employee >().WithMany().HasForeignKey(lb => lb.EmployeeId);
            entity.HasOne< LeaveType >().WithMany().HasForeignKey(lb => lb.LeaveTypeId);
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasOne< Employee >().WithMany().HasForeignKey(lr => lr.EmployeeId);
            entity.HasOne< LeaveType >().WithMany().HasForeignKey(lr => lr.LeaveTypeId);
            entity.HasOne<Employee>().WithMany().HasForeignKey(lr => lr.ManagerId);
            entity.HasIndex(lr => lr.EmployeeId);
            entity.HasIndex(lr => lr.Status);
        });

        modelBuilder.Entity<LeaveRequestApproval>(entity =>
        {
            entity.HasOne<LeaveRequest>().WithMany().HasForeignKey(a => a.LeaveRequestId);
            entity.HasOne<Employee>().WithMany().HasForeignKey(a => a.ApproverEmployeeId);
            entity.HasIndex(a => a.LeaveRequestId);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(a => a.EntityName).IsRequired().HasMaxLength(200);
        });

        base.OnModelCreating(modelBuilder);
    }
}

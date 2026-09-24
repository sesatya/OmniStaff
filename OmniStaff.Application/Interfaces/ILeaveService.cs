using OmniStaff.Application.Dtos;

namespace OmniStaff.Application.Interfaces;

public interface ILeaveService
{
    Task<LeaveRequestDto> ApplyAsync(CreateLeaveRequestDto dto);
    Task<bool> ApproveAsync(Guid requestId, ApproveLeaveDto dto);
    // Additional methods like GetForManagerAsync, ApproveAsync could be added later
}

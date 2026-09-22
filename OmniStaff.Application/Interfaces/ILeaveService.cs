using OmniStaff.Application.Dtos;

namespace OmniStaff.Application.Interfaces;

public interface ILeaveService
{
    Task<LeaveRequestDto> ApplyAsync(CreateLeaveRequestDto dto);
    // Additional methods like GetForManagerAsync, ApproveAsync could be added later
}

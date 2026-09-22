using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;

namespace OmniStaff.Api.Controllers;

[ApiController]
[Route("api/leaverequests")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveRequestsController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Apply([FromBody] CreateLeaveRequestDto dto)
    {
        var created = await _leaveService.ApplyAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, [FromServices] ILeaveRepository leaveRepo)
    {
        var req = await leaveRepo.GetByIdAsync(id);
        if (req is null) return NotFound();
        return Ok(new LeaveRequestDto(req.Id, req.EmployeeId, req.LeaveTypeId, req.StartDate, req.EndDate, req.TotalDays, req.Status.ToString(), req.ManagerId, req.ManagerComment, req.CreatedAtUtc));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    [HttpGet("manager/{managerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByManager(Guid managerId, [FromServices] ILeaveRepository leaveRepo)
    {
        var list = await leaveRepo.GetByManagerAsync(managerId);
        var dtoList = list.Select(req => new LeaveRequestDto(req.Id, req.EmployeeId, req.LeaveTypeId, req.StartDate, req.EndDate, req.TotalDays, req.Status.ToString(), req.ManagerId, req.ManagerComment, req.CreatedAtUtc));
        return Ok(dtoList);
    }

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveLeaveDto dto)
    {
        var ok = await _leaveService.ApproveAsync(id, dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpGet("myleaves")]
    [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyRequests([FromServices] IUserRepository userRepo, [FromServices] IEmployeeRepository employeeRepo, [FromServices] ILeaveRepository leaveRepo)
    {
        // Prefer explicit email claim types: claim 'email', ClaimTypes.Email, then fallback to 'sub' or Name
        var emailClaim = User.Claims.FirstOrDefault(c =>
            string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)
            || string.Equals(c.Type, "email", StringComparison.OrdinalIgnoreCase)
            || string.Equals(c.Type, JwtRegisteredClaimNames.Email, StringComparison.OrdinalIgnoreCase)
            || string.Equals(c.Type, JwtRegisteredClaimNames.Sub, StringComparison.OrdinalIgnoreCase));

        var email = emailClaim?.Value ?? User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var user = await userRepo.GetByEmailAsync(email);
        if (user is null) return NotFound();

        var employee = await employeeRepo.GetByUserIdAsync(user.Id);
        if (employee is null) return NotFound();

        var requests = await leaveRepo.GetByEmployeeAsync(employee.Id);
        var dtoList = requests.Select(req => new LeaveRequestDto(req.Id, req.EmployeeId, req.LeaveTypeId, req.StartDate, req.EndDate, req.TotalDays, req.Status.ToString(), req.ManagerId, req.ManagerComment, req.CreatedAtUtc));
        return Ok(dtoList);
    }
}

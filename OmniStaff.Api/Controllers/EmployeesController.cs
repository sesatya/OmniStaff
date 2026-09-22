using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;

namespace OmniStaff.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _service.GetAllAsync();
        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _service.GetByIdAsync(id);
        if (employee is null) return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        var ok = await _service.UpdateAsync(id, dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}

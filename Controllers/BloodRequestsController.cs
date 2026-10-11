using Microsoft.AspNetCore.Mvc;
using MobileBackEnd.DTOs;
using MobileBackEnd.Services;

namespace MobileBackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BloodRequestsController : ControllerBase
{
    private readonly IBloodRequestService _service;

    public BloodRequestsController(IBloodRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BloodRequestDto>>> GetAll([FromQuery] string? bloodGroup = null, [FromQuery] string? status = null)
    {
        var requests = await _service.GetRequestsAsync(bloodGroup, status);
        return Ok(requests);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BloodRequestDto>> GetById(int id)
    {
        var request = await _service.GetRequestByIdAsync(id);
        if (request == null) return NotFound(new { message = $"Blood request with ID {id} not found." });
        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<BloodRequestDto>> Create([FromBody] CreateBloodRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _service.CreateRequestAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
    {
        var success = await _service.UpdateStatusAsync(id, newStatus);
        if (!success) return NotFound(new { message = $"Blood request with ID {id} not found." });
        return NoContent();
    }
}

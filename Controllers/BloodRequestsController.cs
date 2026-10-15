using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileBackEnd.DTOs;
using MobileBackEnd.Extensions;
using MobileBackEnd.Services;

namespace MobileBackEnd.Controllers;

[ApiController]
[Authorize]
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

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<BloodRequestDto>>> GetMine()
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        return Ok(await _service.GetMyRequestsAsync(uid));
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
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        var created = await _service.CreateRequestAsync(uid, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BloodRequestDto>> Update(int id, [FromBody] CreateBloodRequestDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        return ToActionResult(await _service.UpdateRequestAsync(uid, id, dto), id);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<BloodRequestDto>> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        return ToActionResult(await _service.UpdateStatusAsync(uid, id, dto.Status), id);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        var result = await _service.DeleteRequestAsync(uid, id);
        return result.Status switch
        {
            OperationStatus.Ok => NoContent(),
            OperationStatus.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only delete your own requests." }),
            _ => NotFound(new { message = $"Blood request with ID {id} not found." })
        };
    }

    private ActionResult<BloodRequestDto> ToActionResult(OperationResult<BloodRequestDto> result, int id) =>
        result.Status switch
        {
            OperationStatus.Ok => Ok(result.Value),
            OperationStatus.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only modify your own requests." }),
            OperationStatus.Invalid => BadRequest(new { message = result.Message }),
            _ => NotFound(new { message = $"Blood request with ID {id} not found." })
        };
}

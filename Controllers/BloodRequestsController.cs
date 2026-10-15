using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileBackEnd.DTOs;
using MobileBackEnd.Extensions;
using MobileBackEnd.Services;

namespace MobileBackEnd.Controllers;

/// <summary>
/// API de solicitudes de sangre (api/BloodRequests). Todas las rutas exigen un token de Firebase válido;
/// el usuario sale del token, así nadie puede actuar en nombre de otra persona.
/// </summary>
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

    /// <summary>GET api/BloodRequests?bloodGroup=A+&amp;status=Active : lista las solicitudes, las más recientes primero.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BloodRequestDto>>> GetAll([FromQuery] string? bloodGroup = null, [FromQuery] string? status = null)
    {
        var requests = await _service.GetRequestsAsync(bloodGroup, status);
        return Ok(requests);
    }

    /// <summary>GET api/BloodRequests/mine : solicitudes publicadas por el usuario autenticado.</summary>
    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<BloodRequestDto>>> GetMine()
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        return Ok(await _service.GetMyRequestsAsync(uid));
    }

    /// <summary>GET api/BloodRequests/5 : detalle de una solicitud (404 si no existe).</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BloodRequestDto>> GetById(int id)
    {
        var request = await _service.GetRequestByIdAsync(id);
        if (request == null) return NotFound(new { message = $"Blood request with ID {id} not found." });
        return Ok(request);
    }

    /// <summary>POST api/BloodRequests : publica una solicitud; el solicitante es el usuario del token (201).</summary>
    [HttpPost]
    public async Task<ActionResult<BloodRequestDto>> Create([FromBody] CreateBloodRequestDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        var created = await _service.CreateRequestAsync(uid, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>PUT api/BloodRequests/5 : edita una solicitud propia (403 si es de otro usuario).</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BloodRequestDto>> Update(int id, [FromBody] CreateBloodRequestDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        return ToActionResult(await _service.UpdateRequestAsync(uid, id, dto), id);
    }

    /// <summary>PATCH api/BloodRequests/5/status : cambia el estado (Active, Fulfilled o Cancelled); 400 si es otro valor.</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<BloodRequestDto>> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        return ToActionResult(await _service.UpdateStatusAsync(uid, id, dto.Status), id);
    }

    /// <summary>DELETE api/BloodRequests/5 : elimina una solicitud propia (204).</summary>
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

    /// <summary>Traduce el resultado del servicio a la respuesta HTTP: 200, 400, 403 o 404.</summary>
    private ActionResult<BloodRequestDto> ToActionResult(OperationResult<BloodRequestDto> result, int id) =>
        result.Status switch
        {
            OperationStatus.Ok => Ok(result.Value),
            OperationStatus.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only modify your own requests." }),
            OperationStatus.Invalid => BadRequest(new { message = result.Message }),
            _ => NotFound(new { message = $"Blood request with ID {id} not found." })
        };
}

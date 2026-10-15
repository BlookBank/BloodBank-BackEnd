using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileBackEnd.DTOs;
using MobileBackEnd.Extensions;
using MobileBackEnd.Services;

namespace MobileBackEnd.Controllers;

/// <summary>
/// API de donantes (api/Donors). Todas las rutas exigen un token de Firebase válido y cada usuario
/// solo puede modificar su propio registro de donante.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DonorsController : ControllerBase
{
    private readonly IDonorService _service;

    public DonorsController(IDonorService service)
    {
        _service = service;
    }

    /// <summary>GET api/Donors?bloodGroup=O+&amp;isAvailable=true : lista donantes con filtros opcionales.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonorDto>>> GetAll([FromQuery] string? bloodGroup = null, [FromQuery] bool? isAvailable = null)
    {
        var donors = await _service.GetDonorsAsync(bloodGroup, isAvailable);
        return Ok(donors);
    }

    /// <summary>GET api/Donors/me : el registro de donante del usuario autenticado (404 si no está registrado).</summary>
    [HttpGet("me")]
    public async Task<ActionResult<DonorDto>> GetMine()
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        var donor = await _service.GetDonorByUserIdAsync(uid);
        if (donor == null) return NotFound(new { message = "You are not registered as a donor yet." });
        return Ok(donor);
    }

    /// <summary>GET api/Donors/5 : detalle de un donante.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DonorDto>> GetById(int id)
    {
        var donor = await _service.GetDonorByIdAsync(id);
        if (donor == null) return NotFound(new { message = $"Donor with ID {id} not found." });
        return Ok(donor);
    }

    /// <summary>GET api/Donors/user/{uid} : el registro de donante de un usuario concreto.</summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<DonorDto>> GetByUserId(string userId)
    {
        var donor = await _service.GetDonorByUserIdAsync(userId);
        if (donor == null) return NotFound(new { message = $"Donor for user {userId} not found." });
        return Ok(donor);
    }

    /// <summary>Registra al usuario autenticado como donante (o actualiza su registro si ya existe).</summary>
    [HttpPost]
    public async Task<ActionResult<DonorDto>> Register([FromBody] CreateDonorDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        var donor = await _service.RegisterDonorAsync(uid, dto);
        return CreatedAtAction(nameof(GetById), new { id = donor.Id }, donor);
    }

    /// <summary>PUT api/Donors/5 : edita el registro propio (403 si es de otro usuario).</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<DonorDto>> Update(int id, [FromBody] CreateDonorDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        return ToActionResult(await _service.UpdateDonorAsync(uid, id, dto), id);
    }

    /// <summary>PATCH api/Donors/5/availability : marca al donante como disponible o no disponible.</summary>
    [HttpPatch("{id:int}/availability")]
    public async Task<ActionResult<DonorDto>> SetAvailability(int id, [FromBody] UpdateAvailabilityDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        return ToActionResult(await _service.SetAvailabilityAsync(uid, id, dto.IsAvailable), id);
    }

    /// <summary>DELETE api/Donors/5 : elimina el registro propio (204).</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        var result = await _service.DeleteDonorAsync(uid, id);
        return result.Status switch
        {
            OperationStatus.Ok => NoContent(),
            OperationStatus.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only delete your own donor profile." }),
            _ => NotFound(new { message = $"Donor with ID {id} not found." })
        };
    }

    /// <summary>Traduce el resultado del servicio a la respuesta HTTP: 200, 403 o 404.</summary>
    private ActionResult<DonorDto> ToActionResult(OperationResult<DonorDto> result, int id) =>
        result.Status switch
        {
            OperationStatus.Ok => Ok(result.Value),
            OperationStatus.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only modify your own donor profile." }),
            _ => NotFound(new { message = $"Donor with ID {id} not found." })
        };
}

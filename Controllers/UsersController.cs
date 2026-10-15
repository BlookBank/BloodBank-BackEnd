using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileBackEnd.DTOs;
using MobileBackEnd.Extensions;
using MobileBackEnd.Services;

namespace MobileBackEnd.Controllers;

/// <summary>
/// API de usuarios (api/Users). Exige un token de Firebase válido y cada persona solo puede leer
/// o modificar su propio perfil.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    /// <summary>GET api/Users/me : perfil del usuario autenticado (404 si aún no se sincronizo).</summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMe()
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        var user = await _service.GetUserByIdAsync(uid);
        if (user == null) return NotFound(new { message = "Your profile has not been synced yet." });
        return Ok(user);
    }

    /// <summary>GET api/Users/{uid} : perfil por UID; solo permitido para el propio usuario (403 en otro caso).</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfileDto>> GetById(string id)
    {
        if (id != User.GetUid()) return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only read your own profile." });

        var user = await _service.GetUserByIdAsync(id);
        if (user == null) return NotFound(new { message = $"User with ID {id} not found." });
        return Ok(user);
    }

    /// <summary>Crea o actualiza en SQL Server el usuario autenticado en Firebase.</summary>
    [HttpPost("sync")]
    public async Task<ActionResult<UserProfileDto>> Sync([FromBody] SyncUserDto dto)
    {
        var uid = User.GetUid();
        if (uid == null) return Unauthorized();

        var result = await _service.SyncUserAsync(uid, dto);
        return result.Status switch
        {
            OperationStatus.Ok => Ok(result.Value),
            OperationStatus.Conflict => Conflict(new { message = result.Message }),
            _ => BadRequest(new { message = result.Message })
        };
    }

    /// <summary>PUT api/Users/{uid} : actualiza el perfil propio (actualizacion parcial).</summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserProfileDto>> Update(string id, [FromBody] UpdateUserProfileDto dto)
    {
        if (id != User.GetUid()) return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only modify your own profile." });

        var updated = await _service.UpdateUserAsync(id, dto);
        if (updated == null) return NotFound(new { message = $"User with ID {id} not found." });
        return Ok(updated);
    }
}

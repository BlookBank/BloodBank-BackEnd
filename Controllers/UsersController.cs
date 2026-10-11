using Microsoft.AspNetCore.Mvc;
using MobileBackEnd.DTOs;
using MobileBackEnd.Services;

namespace MobileBackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfileDto>> GetById(string id)
    {
        var user = await _service.GetUserByIdAsync(id);
        if (user == null) return NotFound(new { message = $"User with ID {id} not found." });
        return Ok(user);
    }

    [HttpPost("sync")]
    public async Task<ActionResult<UserProfileDto>> Sync([FromBody] SyncUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var synced = await _service.SyncUserAsync(dto);
        return Ok(synced);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserProfileDto>> Update(string id, [FromBody] UpdateUserProfileDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateUserAsync(id, dto);
        if (updated == null) return NotFound(new { message = $"User with ID {id} not found." });
        return Ok(updated);
    }
}

using Microsoft.AspNetCore.Mvc;
using MobileBackEnd.DTOs;
using MobileBackEnd.Services;

namespace MobileBackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonorsController : ControllerBase
{
    private readonly IDonorService _service;

    public DonorsController(IDonorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonorDto>>> GetAll([FromQuery] string? bloodGroup = null, [FromQuery] bool? isAvailable = null)
    {
        var donors = await _service.GetDonorsAsync(bloodGroup, isAvailable);
        return Ok(donors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DonorDto>> GetById(int id)
    {
        var donor = await _service.GetDonorByIdAsync(id);
        if (donor == null) return NotFound(new { message = $"Donor with ID {id} not found." });
        return Ok(donor);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<DonorDto>> GetByUserId(string userId)
    {
        var donor = await _service.GetDonorByUserIdAsync(userId);
        if (donor == null) return NotFound(new { message = $"Donor for user {userId} not found." });
        return Ok(donor);
    }

    [HttpPost]
    public async Task<ActionResult<DonorDto>> Register([FromBody] CreateDonorDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _service.RegisterDonorAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

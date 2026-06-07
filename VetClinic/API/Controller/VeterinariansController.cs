using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;

namespace VetClinic.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class VeterinariansController : ControllerBase
{
    private readonly IVeterinariansService _veterinariansService;
    
    public VeterinariansController(IVeterinariansService veterinariansService)
    {
        _veterinariansService = veterinariansService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateVeterinarianAsync([FromBody] CreateVeterinarianRequestDto dto)
    {
        try
        {
            var id = await _veterinariansService.CreateVeterinarianAsync(dto);
            
            return CreatedAtAction("GetVeterinarianById", new { id }, new { id });
        }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        catch (ArgumentException ex)         { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteVeterinarianAsync(Guid id)
    {
        try
        {
            await _veterinariansService.DeleteVeterinarianAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVeterinarianByIdAsync(Guid id)
    {
        return Ok(new { id });
    }
}
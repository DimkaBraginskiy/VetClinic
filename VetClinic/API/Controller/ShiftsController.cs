using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;

namespace VetClinic.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly IShiftsService _shiftsService;

    public ShiftsController(IShiftsService shiftsService)
    {
        _shiftsService = shiftsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateShiftAsync([FromBody] CreateShiftRequestDto dto)
    {
        try
        {
            var id = await _shiftsService.CreateShiftAsync(dto);
            return Created($"/api/shifts/{id}", new { id });
        }
        catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
        catch (ArgumentException ex)         { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteShiftAsync(Guid id)
    {
        try
        {
            await _shiftsService.DeleteShiftAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (ArgumentException ex)    { return BadRequest(ex.Message); }
    }
}
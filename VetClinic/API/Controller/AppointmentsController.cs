using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Service;

namespace VetClinic.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentsService _appointmentsService;

    public AppointmentsController(AppointmentsService appointmentsService)
    {
        _appointmentsService = appointmentsService;
    }

    [HttpPost("online")]
    public async Task<IActionResult> ScheduleOnline([FromBody] OnlineAppointmentRequestDto dto)
    {
        try
        {
            var result = await _appointmentsService.ScheduleOnlineAsync(dto);
            return CreatedAtAction(nameof(ScheduleOnline), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
        catch (ArgumentException ex)         { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
    }
}
using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;

namespace VetClinic.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentsService;

    public AppointmentsController(IAppointmentService appointmentsService)
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
    
    [HttpPost("home")]
    public async Task<IActionResult> ScheduleHome([FromBody] HomeAppointmentRequestDto dto)
    {
        try
        {
            var result = await _appointmentsService.ScheduleHomeAsync(dto);
            return CreatedAtAction(nameof(ScheduleHome), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
        catch (ArgumentException ex)         { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
    }
    
    [HttpPost("clinic")]
    public async Task<IActionResult> ScheduleClinic([FromBody] ClinicAppointmentRequestDto dto)
    {
        try
        {
            var result = await _appointmentsService.ScheduleClinicAsync(dto);
            return CreatedAtAction(nameof(ScheduleClinic), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
        catch (ArgumentException ex)         { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerAppointments([FromQuery] Guid customerId)
    {
        try
        {
            var result = await _appointmentsService.GetCustomerAppointmentsAsync(customerId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAppointment(Guid id, [FromQuery] Guid customerId)
    {
        try
        {
            await _appointmentsService.DeleteAppointmentAsync(id, customerId);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpGet("validate-discount")]
    public async Task<IActionResult> ValidateDiscountAsync([FromQuery] string code)
    {
        try
        {
            var result = await _appointmentsService.ValidateDiscountAsync(code);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpPut("cancel/{id:guid}")]
    public async Task<IActionResult> CancelAppointmentAsync(Guid id)
    {
        try
        {
            var result = await _appointmentsService.CancelAppointmentAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }
}
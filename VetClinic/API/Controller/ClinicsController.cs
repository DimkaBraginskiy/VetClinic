using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;

namespace VetClinic.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class ClinicsController : ControllerBase
{
    private readonly IClinicService _clinicService;

    public ClinicsController(IClinicService clinicService)
    {
        _clinicService = clinicService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClinics()
    {
        var result = await _clinicService.GetAllClinicsAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClinic([FromBody] CreateClinicRequestDto dto)
    {
        try
        {
            var id = await _clinicService.CreateClinicAsync(dto);
            return CreatedAtAction(nameof(CreateClinic), new { id }, new { id });
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPost("{clinicId}/cabinets")]
    public async Task<IActionResult> AddCabinet(Guid clinicId, [FromBody] AddCabinetRequestDto dto)
    {
        try
        {
            var id = await _clinicService.AddCabinetAsync(clinicId, dto);
            return CreatedAtAction(nameof(AddCabinet), new { clinicId, id }, new { id });
        }
        catch (KeyNotFoundException ex)  { return NotFound(ex.Message); }
        catch (ArgumentException ex)     { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
    }
}
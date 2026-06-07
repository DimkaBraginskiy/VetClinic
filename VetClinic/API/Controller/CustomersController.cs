using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;

namespace VetClinic.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomersService _customersService;

    public CustomersController(ICustomersService customersService)
    {
        _customersService = customersService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequestDto dto)
    {
        try
        {
            var id = await _customersService.CreateCustomerAsync(dto);
            return CreatedAtAction(nameof(CreateCustomer), new { id }, new { id });
        }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        catch (ArgumentException ex)         { return BadRequest(ex.Message); }
    }

    [HttpGet("{customerId}/loyalty-points")]
    public async Task<IActionResult> GetLoyaltyPoints(Guid customerId)
    {
        try
        {
            var points = await _customersService.GetLoyaltyPointsAsync(customerId);
            return Ok(new { loyaltyPoints = points });
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpGet("{customerId}/appointments/count")]
    public async Task<IActionResult> GetAppointmentCount(Guid customerId)
    {
        try
        {
            var count = await _customersService.GetAppointmentCountAsync(customerId);
            return Ok(new { count });
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpGet("{customerId}/animals/{animalId:guid}")]
    public async Task<IActionResult> GetAnimalById(Guid customerId, Guid animalId)
    {
        try
        {
            var result = await _customersService.GetAnimalByIdAsync(customerId, animalId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpGet("{customerId}/animals")]
    public async Task<IActionResult> GetCustomerAnimals(Guid customerId)
    {
        try
        {
            var result = await _customersService.GetCustomerAnimalsAsync(customerId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpDelete("{customerId}/animals/{animalId}")]
    public async Task<IActionResult> DeleteCustomerAnimal(Guid customerId, Guid animalId)
    {
        try
        {
            await _customersService.DeleteCustomerAnimalAsync(customerId, animalId);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpPost("{customerId}/animals")]
    public async Task<IActionResult> CreateCustomerAnimal([FromBody] CreatAnimalRequestDto dto, Guid customerId)
    {
        try
        {
            var id = await _customersService.CreateCustomerAnimalAsync(dto, customerId);
            return CreatedAtAction(nameof(GetCustomerAnimals), new { customerId }, new { id });
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (ArgumentException ex)    { return BadRequest(ex.Message); }
    }
}
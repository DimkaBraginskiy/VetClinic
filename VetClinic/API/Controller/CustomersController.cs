using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.Services;

namespace VetClinic.API.Controller;

public class CustomersController : ControllerBase
{
    private readonly ICustomersService _customersService;

    public CustomersController(ICustomersService customersService)
    {
        _customersService = customersService;
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
}
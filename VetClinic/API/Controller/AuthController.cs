using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;

namespace VetClinic.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration   _config;
    private readonly ICustomersService _customers;

    public AuthController(IConfiguration config, ICustomersService customers)
    {
        _config    = config;
        _customers = customers;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var expectedEmail    = _config["Auth:Email"];
        var expectedPassword = _config["Auth:Password"];

        if (dto.Email != expectedEmail || dto.Password != expectedPassword)
            return Unauthorized(new LoginResponseDto(false, Message: "Invalid email or password."));

        var customerId = await _customers.GetCustomerIdByEmailAsync(dto.Email);

        return Ok(new LoginResponseDto(true, customerId));
    }
}
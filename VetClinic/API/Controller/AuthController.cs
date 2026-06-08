using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;
using VetClinic.Application.Interface;

namespace VetClinic.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration      _config;
    private readonly ICustomersService   _customers;
    private readonly IVeterinariansService _vets;

    public AuthController(IConfiguration config, ICustomersService customers, IVeterinariansService vets)
    {
        _config    = config;
        _customers = customers;
        _vets      = vets;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var users = _config.GetSection("Auth:Users").Get<AuthUserConfig[]>() ?? [];
        var match = Array.Find(users, u => u.Email == dto.Email && u.Password == dto.Password);

        if (match is null)
            return Unauthorized(new LoginResponseDto(false, Message: "Invalid email or password."));

        var customerId = await _customers.GetCustomerIdByEmailAsync(dto.Email);
        if (customerId is not null)
            return Ok(new LoginResponseDto(true, CustomerId: customerId));

        var vetId = await _vets.GetVetIdByEmailAsync(dto.Email);
        return Ok(new LoginResponseDto(true, VeterinarianId: vetId));
    }

    private sealed record AuthUserConfig(string Email = "", string Password = "");
}
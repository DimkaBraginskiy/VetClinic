namespace VetClinic.Application.DTOs;

public record LoginResponseDto(bool Success, Guid? CustomerId = null, string? Message = null);
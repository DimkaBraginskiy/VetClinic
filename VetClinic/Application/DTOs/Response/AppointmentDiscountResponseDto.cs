namespace VetClinic.Application.DTOs;

public record AppointmentDiscountResponseDto(
    Guid Id,
    decimal Percentage,
    string PromoCode
);
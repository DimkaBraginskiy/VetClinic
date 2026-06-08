namespace VetClinic.Application.DTOs.Response;

public record AppointmentDiscountResponseDto(
    Guid Id,
    decimal Percentage,
    string PromoCode
);
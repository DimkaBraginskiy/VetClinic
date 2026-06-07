using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;

namespace VetClinic.Application.Services;

public interface IAppointmentService
{
    public Task<ScheduledAppointmentResponseDto> ScheduleOnlineAsync(OnlineAppointmentRequestDto dto);
    public Task<ScheduledAppointmentResponseDto> ScheduleHomeAsync(HomeAppointmentRequestDto dto);
    public Task<ScheduledAppointmentResponseDto> ScheduleClinicAsync(ClinicAppointmentRequestDto dto);
    public Task<DiscountInfoDto> ValidateDiscountAsync(string promoCode);
    public Task<List<ScheduledAppointmentResponseDto>> GetCustomerAppointmentsAsync(Guid customerId);
    public Task DeleteAppointmentAsync(Guid appointmentId, Guid customerId);
    public Task<Guid> CancelAppointmentAsync(Guid appointmentId);
}
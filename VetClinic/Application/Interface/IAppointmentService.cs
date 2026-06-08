using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;

namespace VetClinic.Application.Interface;

public interface IAppointmentService
{
    public Task<ScheduledAppointmentResponseDto> ScheduleOnlineAsync(OnlineAppointmentRequestDto dto);
    public Task<ScheduledAppointmentResponseDto> ScheduleHomeAsync(HomeAppointmentRequestDto dto);
    public Task<ScheduledAppointmentResponseDto> ScheduleClinicAsync(ClinicAppointmentRequestDto dto);
    public Task<DiscountInfoDto> ValidateDiscountAsync(string promoCode);
    public Task<List<AppointmentMimimalResponseDto>> GetCustomerAppointmentsAsync(Guid customerId, Guid? animalId = null);
    public Task<ScheduledAppointmentResponseDto> GetAppointmentByIdAsync(Guid appointmentId, Guid customerId);
    public Task<List<AppointmentMimimalResponseDto>> GetVeterinarianAppointmentsAsync(Guid veterinarianId);
    public Task<ScheduledAppointmentResponseDto> GetVeterinarianAppointmentByIdAsync(Guid appointmentId, Guid veterinarianId);
    public Task DeleteAppointmentAsync(Guid appointmentId, Guid customerId);
    public Task<Guid> CancelAppointmentAsync(Guid appointmentId);
}
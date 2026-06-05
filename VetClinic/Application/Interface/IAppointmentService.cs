using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;

namespace VetClinic.Application.Services;

public interface IAppointmentService
{
    public Task<ScheduledAppointmentResponseDto> ScheduleOnlineAsync(OnlineAppointmentRequestDto dto);
    public Task<ScheduledAppointmentResponseDto> ScheduleHomeAsync(HomeAppointmentRequestDto dto);
    public Task<ScheduledAppointmentResponseDto> ScheduleClinicAsync(ClinicAppointmentRequestDto dto);
}
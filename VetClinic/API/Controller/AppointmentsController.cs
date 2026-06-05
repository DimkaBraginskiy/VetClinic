using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.Service;
using VetClinic.Infrastructure;

namespace VetClinic.API.Controller;

public class AppointmentsController : ControllerBase
{
    private readonly AppointmentsService _appointmentsService;

    public AppointmentsController(AppointmentsService appointmentsService)
    {
        _appointmentsService = appointmentsService;
    }
    
    
}
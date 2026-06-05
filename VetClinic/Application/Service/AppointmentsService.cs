using VetClinic.Application.DTOs;
using VetClinic.Infrastructure;

namespace VetClinic.Application.Service;

public class AppointmentsService
{
    private readonly AppDbContext _context;

    public AppointmentsService(AppDbContext context)
    {
        _context = context;
    }
}
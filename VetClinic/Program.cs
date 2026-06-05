// using VetClinic.Domain.Entities;
//
// var builder = WebApplication.CreateBuilder(args);
//
// builder.Services.AddOpenApi();
//
// var app = builder.Build();
//
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
//
// app.UseHttpsRedirection();
//
//
//
// app.Run();

using Microsoft.AspNetCore.Components.Sections;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;

var customer = new Customer(
    "Dimka",
    "Dimkovich",
    "dimka@gmail.com",
    100,
    null
    );



var clinic = new Clinic(
    new Address("USA", "Akron", "Springfield", 2),
    1,
    101
    );

var veterinarian = new Veterinarian(
    VeterinarianType.Surgeon,
    12000,
    DateTime.Now.AddYears(-2),
    new List<TreatmentType> { TreatmentType.Surgery},
    clinic,
    "Skyler",
    "White",
    "white@gmail.com",
    null
    );

var treatment = new Treatment(
    TreatmentType.Surgery,
    200,
    2500,
    veterinarian
    );

var animal = new Animal(
    "Ilan",
    DateTime.Now.AddYears(-3),
    7.6m,
        AnimalSpecies.Dog,
    "Sheltie",
    customer
    );

var appointment = Appointment.CreateClinic(
    type: AppointmentType.Treatment,
    startDate: DateTime.Now.AddDays(1),
    250,
    customer,
    veterinarian,
        animal,
    DateTime.Now.AddDays(1).AddMinutes(-15),
    clinic.Cabinets.First(),
    treatment
    );

Console.WriteLine(appointment.ToString());

//    appointment.ChangeToClinic(DateTime.Now.AddDays(1).AddMinutes(-15), clinic.Cabinets.First());
var homeAddress = new Address("USA", "Akron", "Springfield", 2);
appointment.ChangeToHome(homeAddress);

Console.WriteLine(appointment.ToString());



    
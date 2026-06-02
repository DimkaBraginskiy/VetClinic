using VetClinic.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

Person person = new Customer("john","doe",null,"ok@mgail.cok",120);

Console.WriteLine("Wassuspppppppppp");

Console.WriteLine(person);

app.Run();
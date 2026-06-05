namespace VetClinic.Application.DTOs;

public record CustomerAnimalResponseDto
{ 
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal? Weight { get; init; }
    public string? Species { get; init; }
    public string? Breed { get; init; }
}
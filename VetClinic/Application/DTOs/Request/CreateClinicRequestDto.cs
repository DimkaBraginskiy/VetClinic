namespace VetClinic.Application.DTOs.Request;

public class CreateClinicRequestDto
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public int BuildingNumber { get; set; }
    public string PostalCode { get; set; }
    public int InitialCabinetFloor  { get; set; }
    public int InitialCabinetNumber { get; set; }
}
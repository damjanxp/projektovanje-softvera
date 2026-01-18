namespace TourApp.Application.Tours.DTOs;

public class AddKeyPointRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

namespace TourApp.Application.Cancellations.DTOs;

public class CancellationResultDto
{
    public int ToursCanceled { get; set; }
    public int TouristsRefunded { get; set; }
    public int TotalBonusPointsAwarded { get; set; }
    public List<CanceledTourDto> CanceledTours { get; set; } = new();
}

public class CanceledTourDto
{
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int TouristsAffected { get; set; }
    public int BonusPointsAwarded { get; set; }
}

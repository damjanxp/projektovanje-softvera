namespace TourApp.Application.Notifications.DTOs;

public class ReminderResultDto
{
    public int TourCount { get; set; }
    public int RemindersSent { get; set; }
    public List<TourReminderDto> Tours { get; set; } = new();
}

public class TourReminderDto
{
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int TouristsNotified { get; set; }
}

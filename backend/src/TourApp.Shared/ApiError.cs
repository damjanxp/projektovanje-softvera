namespace TourApp.Shared;

public class ApiError
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }

    public ApiError(string code, string message, string? details = null)
    {
        Code = code;
        Message = message;
        Details = details;
    }
}

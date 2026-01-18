namespace TourApp.Shared;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }

    private ApiResponse(bool success, T? data, ApiError? error)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    public static ApiResponse<T> Ok(T data)
    {
        return new ApiResponse<T>(true, data, null);
    }

    public static ApiResponse<T> Fail(string code, string message, string? details = null)
    {
        return new ApiResponse<T>(false, default, new ApiError(code, message, details));
    }
}

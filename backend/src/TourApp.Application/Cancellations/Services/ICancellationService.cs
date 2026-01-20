using TourApp.Application.Cancellations.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Cancellations.Services;

public interface ICancellationService
{
    Task<ApiResponse<CancellationResultDto>> CancelToursWithoutReplacementAsync();
}

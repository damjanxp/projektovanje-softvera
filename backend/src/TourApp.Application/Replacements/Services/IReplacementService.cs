using TourApp.Application.Replacements.DTOs;
using TourApp.Application.Tours.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Replacements.Services;

public interface IReplacementService
{
    Task<ApiResponse<TourDto>> RequestReplacementAsync(Guid guideId, Guid tourId);
    Task<ApiResponse<TourDto>> CancelReplacementRequestAsync(Guid guideId, Guid tourId);
    Task<ApiResponse<List<ReplacementTourDto>>> GetReplacementToursAsync(Guid guideId);
    Task<ApiResponse<TourDto>> TakeOverTourAsync(Guid newGuideId, Guid tourId);
}

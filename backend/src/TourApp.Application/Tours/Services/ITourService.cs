using TourApp.Application.Tours.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Tours.Services;

public interface ITourService
{
    Task<ApiResponse<TourDto>> CreateTourAsync(Guid guideId, CreateTourRequest request);
    Task<ApiResponse<KeyPointDto>> AddKeyPointAsync(Guid guideId, Guid tourId, AddKeyPointRequest request);
    Task<ApiResponse<TourDto>> PublishTourAsync(Guid guideId, Guid tourId);
    Task<ApiResponse<List<TourDto>>> GetMyToursAsync(Guid guideId, bool sortAscending = true);
    Task<ApiResponse<List<TourDto>>> GetPublishedToursAsync(bool sortAscending = true);
    Task<ApiResponse<TourDto>> GetTourByIdAsync(Guid tourId, Guid? userId = null);
}

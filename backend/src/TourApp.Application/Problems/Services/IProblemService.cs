using TourApp.Application.Problems.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Problems.Services;

public interface IProblemService
{
    Task<ApiResponse<ProblemDto>> CreateProblemAsync(Guid touristId, CreateProblemRequest request);
    Task<ApiResponse<ProblemDto>> ResolveProblemAsync(Guid problemId, Guid guideId);
    Task<ApiResponse<ProblemDto>> SendToReviewAsync(Guid problemId, Guid guideId);
    Task<ApiResponse<ProblemDto>> RejectProblemAsync(Guid problemId, Guid adminId);
    Task<ApiResponse<ProblemDto>> ReopenProblemAsync(Guid problemId, Guid adminId);
    Task<ApiResponse<ProblemDto>> GetProblemByIdAsync(Guid problemId);
    Task<ApiResponse<List<ProblemDto>>> GetProblemsForTourAsync(Guid tourId);
    Task<ApiResponse<List<ProblemDto>>> GetProblemsInReviewAsync();
    Task<ApiResponse<List<ProblemEventDto>>> GetProblemEventsAsync(Guid problemId);
}

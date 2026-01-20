using TourApp.Application.Problems.DTOs;
using TourApp.Application.Problems.Interfaces;
using TourApp.Application.Purchases.Interfaces;
using TourApp.Application.Purchases.Services;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Problems;
using TourApp.Shared;

namespace TourApp.Application.Problems.Services;

public class ProblemService : IProblemService
{
    private readonly IProblemRepository _problemRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IEmailService _emailService;

    public ProblemService(
        IProblemRepository problemRepository,
        IPurchaseRepository purchaseRepository,
        ITourRepository tourRepository,
        IEmailService emailService)
    {
        _problemRepository = problemRepository;
        _purchaseRepository = purchaseRepository;
        _tourRepository = tourRepository;
        _emailService = emailService;
    }

    public async Task<ApiResponse<ProblemDto>> CreateProblemAsync(Guid touristId, CreateProblemRequest request)
    {
        // Check if tour exists
        var tour = await _tourRepository.GetByIdAsync(request.TourId);
        if (tour == null)
        {
            return ApiResponse<ProblemDto>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        // Check if tourist has purchased the tour
        var purchases = await _purchaseRepository.GetByTouristAsync(touristId);
        var hasPurchased = purchases
            .SelectMany(p => p.PurchasedTours)
            .Any(pt => pt.TourId == request.TourId);

        if (!hasPurchased)
        {
            return ApiResponse<ProblemDto>.Fail("NOT_PURCHASED", "You can only report problems for tours you have purchased.");
        }

        try
        {
            var problem = new Problem(
                Guid.NewGuid(),
                touristId,
                request.TourId,
                request.Title,
                request.Description
            );

            await _problemRepository.AddAsync(problem);
            await _problemRepository.SaveChangesAsync();

            // Notify guide about new problem
            await _emailService.SendProblemNotificationToGuideAsync(tour.GuideId, tour.Name, problem.Title);

            return ApiResponse<ProblemDto>.Ok(MapToDto(problem));
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<ProblemDto>.Fail("VALIDATION_ERROR", ex.Message);
        }
    }

    public async Task<ApiResponse<ProblemDto>> ResolveProblemAsync(Guid problemId, Guid guideId)
    {
        var problem = await _problemRepository.GetByIdAsync(problemId);
        if (problem == null)
        {
            return ApiResponse<ProblemDto>.Fail("PROBLEM_NOT_FOUND", "Problem not found.");
        }

        // Verify guide owns the tour
        var tour = await _tourRepository.GetByIdAsync(problem.TourId);
        if (tour == null || tour.GuideId != guideId)
        {
            return ApiResponse<ProblemDto>.Fail("UNAUTHORIZED", "You are not authorized to resolve this problem.");
        }

        try
        {
            var statusEvent = problem.Resolve(guideId);
            await _problemRepository.AddEventAsync(statusEvent);
            await _problemRepository.SaveChangesAsync();

            return ApiResponse<ProblemDto>.Ok(MapToDto(problem));
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<ProblemDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    public async Task<ApiResponse<ProblemDto>> SendToReviewAsync(Guid problemId, Guid guideId)
    {
        var problem = await _problemRepository.GetByIdAsync(problemId);
        if (problem == null)
        {
            return ApiResponse<ProblemDto>.Fail("PROBLEM_NOT_FOUND", "Problem not found.");
        }

        // Verify guide owns the tour
        var tour = await _tourRepository.GetByIdAsync(problem.TourId);
        if (tour == null || tour.GuideId != guideId)
        {
            return ApiResponse<ProblemDto>.Fail("UNAUTHORIZED", "You are not authorized to send this problem for review.");
        }

        try
        {
            var statusEvent = problem.SendToReview(guideId);
            await _problemRepository.AddEventAsync(statusEvent);
            await _problemRepository.SaveChangesAsync();

            return ApiResponse<ProblemDto>.Ok(MapToDto(problem));
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<ProblemDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    public async Task<ApiResponse<ProblemDto>> RejectProblemAsync(Guid problemId, Guid adminId)
    {
        var problem = await _problemRepository.GetByIdAsync(problemId);
        if (problem == null)
        {
            return ApiResponse<ProblemDto>.Fail("PROBLEM_NOT_FOUND", "Problem not found.");
        }

        try
        {
            var statusEvent = problem.Reject(adminId);
            await _problemRepository.AddEventAsync(statusEvent);
            await _problemRepository.SaveChangesAsync();

            return ApiResponse<ProblemDto>.Ok(MapToDto(problem));
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<ProblemDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    public async Task<ApiResponse<ProblemDto>> ReopenProblemAsync(Guid problemId, Guid adminId)
    {
        var problem = await _problemRepository.GetByIdAsync(problemId);
        if (problem == null)
        {
            return ApiResponse<ProblemDto>.Fail("PROBLEM_NOT_FOUND", "Problem not found.");
        }

        try
        {
            var statusEvent = problem.Reopen(adminId);
            await _problemRepository.AddEventAsync(statusEvent);
            await _problemRepository.SaveChangesAsync();

            return ApiResponse<ProblemDto>.Ok(MapToDto(problem));
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<ProblemDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    public async Task<ApiResponse<ProblemDto>> GetProblemByIdAsync(Guid problemId)
    {
        var problem = await _problemRepository.GetByIdAsync(problemId);
        if (problem == null)
        {
            return ApiResponse<ProblemDto>.Fail("PROBLEM_NOT_FOUND", "Problem not found.");
        }

        return ApiResponse<ProblemDto>.Ok(MapToDto(problem));
    }

    public async Task<ApiResponse<List<ProblemDto>>> GetProblemsForTourAsync(Guid tourId)
    {
        var tour = await _tourRepository.GetByIdAsync(tourId);
        if (tour == null)
        {
            return ApiResponse<List<ProblemDto>>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        var problems = await _problemRepository.GetByTourAsync(tourId);
        var dtos = problems.Select(MapToDto).ToList();

        return ApiResponse<List<ProblemDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<List<ProblemDto>>> GetProblemsInReviewAsync()
    {
        var problems = await _problemRepository.GetProblemsInReviewAsync();
        var dtos = problems.Select(MapToDto).ToList();

        return ApiResponse<List<ProblemDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<List<ProblemEventDto>>> GetProblemEventsAsync(Guid problemId)
    {
        var problem = await _problemRepository.GetByIdAsync(problemId);
        if (problem == null)
        {
            return ApiResponse<List<ProblemEventDto>>.Fail("PROBLEM_NOT_FOUND", "Problem not found.");
        }

        var events = await _problemRepository.GetEventsByProblemAsync(problemId);
        var dtos = events.Select(MapEventToDto).ToList();

        return ApiResponse<List<ProblemEventDto>>.Ok(dtos);
    }

    private static ProblemDto MapToDto(Problem problem)
    {
        return new ProblemDto
        {
            Id = problem.Id,
            TouristId = problem.TouristId,
            TourId = problem.TourId,
            Title = problem.Title,
            Description = problem.Description,
            Status = problem.Status,
            CreatedAt = problem.CreatedAt
        };
    }

    private static ProblemEventDto MapEventToDto(ProblemStatusChangedEvent evt)
    {
        return new ProblemEventDto
        {
            Id = evt.Id,
            ProblemId = evt.ProblemId,
            OldStatus = evt.OldStatus,
            NewStatus = evt.NewStatus,
            ChangedAt = evt.ChangedAt,
            ChangedByRole = evt.ChangedByRole,
            ChangedByUserId = evt.ChangedByUserId
        };
    }
}

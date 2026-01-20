using TourApp.Application.Notifications.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Notifications.Services;

public interface IReminderService
{
    Task<ApiResponse<ReminderResultDto>> SendTourRemindersAsync();
}

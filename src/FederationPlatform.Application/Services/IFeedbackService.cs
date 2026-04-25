using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IFeedbackService
{
    Task<FeedbackDto> CreateFeedbackAsync(CreateFeedbackDto dto);
    Task<FeedbackDto?> GetFeedbackByIdAsync(int id);
    Task<List<FeedbackDto>> GetActivityFeedbacksAsync(int activityId);
    Task<List<FeedbackDto>> GetApprovedFeedbacksAsync(int activityId);
    Task<double> GetAverageRatingAsync(int activityId);
    Task<bool> UpdateFeedbackAsync(UpdateFeedbackDto dto);
    Task<bool> ApproveFeedbackAsync(int id);
    Task<bool> DeleteFeedbackAsync(int id);
    Task<bool> UserHasFeedbackAsync(int userId, int activityId);
}

using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Interfaces;

public interface IFeedbackRepository
{
    Task<Feedback> AddAsync(Feedback feedback);
    Task<Feedback?> GetByIdAsync(int id);
    Task<List<Feedback>> GetActivityFeedbacksAsync(int activityId);
    Task<List<Feedback>> GetApprovedFeedbacksAsync(int activityId);
    Task<double> GetAverageRatingAsync(int activityId);
    Task<bool> UserHasFeedbackAsync(int userId, int activityId);
    Task DeleteAsync(int id);
}

using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Repositories;

public class FeedbackRepository : RepositoryBase<Feedback>, IFeedbackRepository
{
    public FeedbackRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Feedback>> GetActivityFeedbacksAsync(int activityId)
    {
        return await _dbSet
            .Include(f => f.User)
            .ThenInclude(u => u.UserProfile)
            .Include(f => f.Activity)
            .Where(f => f.ActivityId == activityId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Feedback>> GetApprovedFeedbacksAsync(int activityId)
    {
        return await _dbSet
            .Include(f => f.User)
            .ThenInclude(u => u.UserProfile)
            .Include(f => f.Activity)
            .Where(f => f.ActivityId == activityId && f.IsApproved)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<double> GetAverageRatingAsync(int activityId)
    {
        var feedbacks = await _dbSet
            .Where(f => f.ActivityId == activityId && f.IsApproved)
            .ToListAsync();

        if (feedbacks.Count == 0)
            return 0;

        return feedbacks.Average(f => f.Rating);
    }

    public async Task<bool> UserHasFeedbackAsync(int userId, int activityId)
    {
        return await _dbSet
            .AnyAsync(f => f.UserId == userId && f.ActivityId == activityId);
    }
}

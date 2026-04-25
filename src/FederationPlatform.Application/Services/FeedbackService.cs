using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FeedbackDto> CreateFeedbackAsync(CreateFeedbackDto dto)
    {
        // Check if user already has feedback for this activity
        if (await UserHasFeedbackAsync(dto.UserId, dto.ActivityId))
        {
            throw new InvalidOperationException("شما قبلاً برای این فعالیت نظر ثبت کرده‌اید");
        }

        var feedback = new Feedback
        {
            ActivityId = dto.ActivityId,
            UserId = dto.UserId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Feedbacks.AddAsync(feedback);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FeedbackDto>(feedback);
    }

    public async Task<FeedbackDto?> GetFeedbackByIdAsync(int id)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
        return feedback == null ? null : _mapper.Map<FeedbackDto>(feedback);
    }

    public async Task<List<FeedbackDto>> GetActivityFeedbacksAsync(int activityId)
    {
        var feedbacks = await _unitOfWork.Feedbacks.GetActivityFeedbacksAsync(activityId);
        return _mapper.Map<List<FeedbackDto>>(feedbacks);
    }

    public async Task<List<FeedbackDto>> GetApprovedFeedbacksAsync(int activityId)
    {
        var feedbacks = await _unitOfWork.Feedbacks.GetApprovedFeedbacksAsync(activityId);
        return _mapper.Map<List<FeedbackDto>>(feedbacks);
    }

    public async Task<double> GetAverageRatingAsync(int activityId)
    {
        return await _unitOfWork.Feedbacks.GetAverageRatingAsync(activityId);
    }

    public async Task<bool> UpdateFeedbackAsync(UpdateFeedbackDto dto)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(dto.Id);
        if (feedback == null)
            return false;

        feedback.Rating = dto.Rating;
        feedback.Comment = dto.Comment;
        feedback.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveFeedbackAsync(int id)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
        if (feedback == null)
            return false;

        feedback.IsApproved = true;
        feedback.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteFeedbackAsync(int id)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
        if (feedback == null)
            return false;

        await _unitOfWork.Feedbacks.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UserHasFeedbackAsync(int userId, int activityId)
    {
        return await _unitOfWork.Feedbacks.UserHasFeedbackAsync(userId, activityId);
    }
}

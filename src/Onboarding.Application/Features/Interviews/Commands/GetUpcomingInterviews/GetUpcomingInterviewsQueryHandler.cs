// Application/Features/Interviews/Queries/GetUpcomingInterviews/GetUpcomingInterviewsQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Interviews.Queries.GetUpcomingInterviews;

public class GetUpcomingInterviewsQueryHandler : IRequestHandler<GetUpcomingInterviewsQuery, Result<List<UpcomingInterviewDto>>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUpcomingInterviewsQueryHandler> _logger;

    public GetUpcomingInterviewsQueryHandler(
        IInterviewRepository interviewRepository,
        IUserRepository userRepository,
        ILogger<GetUpcomingInterviewsQueryHandler> logger)
    {
        _interviewRepository = interviewRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<UpcomingInterviewDto>>> Handle(
        GetUpcomingInterviewsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<List<UpcomingInterviewDto>>.Failure("User not found");

            List<Interview> interviews;
            var today = DateTime.UtcNow.Date;

            // 2. Get interviews based on user role
            switch (request.UserRole)
            {
                case UserRole.Parent:
                    // Get interviews for parent's children
                    interviews = await GetInterviewsForParent(request.UserId, request.FromDate, request.ToDate, cancellationToken);
                    break;

                case UserRole.Teacher:
                    // Get interviews assigned to this teacher
                    interviews = await GetInterviewsForTeacher(request.UserId, request.FromDate, request.ToDate, cancellationToken);
                    break;

                case UserRole.SuperAdmin:
                case UserRole.SchoolAdmin:
                case UserRole.AdmissionOfficer:
                    // Get all upcoming interviews
                    interviews = await GetAllUpcomingInterviews(request.FromDate, request.ToDate, cancellationToken);
                    break;

                default:
                    return Result<List<UpcomingInterviewDto>>.Failure("User role cannot access interviews");
            }

            // 3. Filter to upcoming interviews only
            interviews = interviews
                .Where(i => i.ScheduledDate >= today && i.Status == InterviewStatus.Scheduled)
                .OrderBy(i => i.ScheduledDate)
                .ThenBy(i => i.ScheduledTime)
                .Take(request.Limit ?? 50)
                .ToList();

            // 4. Map to DTOs
            var interviewDtos = interviews.Select(i => new UpcomingInterviewDto(
                i.Id,
                i.ApplicationId,
                i.Application?.ApplicationNumber ?? "N/A",
                $"{i.Student?.FirstName} {i.Student?.LastName}",
                i.Application?.User != null ? $"{i.Application.User.FirstName} {i.Application.User.LastName}" : "N/A",
                i.ScheduledDate,
                i.ScheduledTime,
                i.Type,
                i.MeetingLink,
                i.InterviewerName,
                i.Status.ToString()
            )).ToList();

            return Result<List<UpcomingInterviewDto>>.Success(interviewDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming interviews for user {UserId}", request.UserId);
            return Result<List<UpcomingInterviewDto>>.Failure("An error occurred while fetching upcoming interviews");
        }
    }

    private async Task<List<Interview>> GetInterviewsForParent(
        Guid parentId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var interviews = await _interviewRepository.GetAllAsync(cancellationToken);

        return interviews
            .Where(i => i.Application?.UserId == parentId)
            .Where(i => !fromDate.HasValue || i.ScheduledDate >= fromDate.Value.Date)
            .Where(i => !toDate.HasValue || i.ScheduledDate <= toDate.Value.Date)
            .ToList();
    }

    private async Task<List<Interview>> GetInterviewsForTeacher(
        Guid teacherId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var interviews = await _interviewRepository.GetByInterviewerIdAsync(teacherId, cancellationToken);

        return interviews
            .Where(i => !fromDate.HasValue || i.ScheduledDate >= fromDate.Value.Date)
            .Where(i => !toDate.HasValue || i.ScheduledDate <= toDate.Value.Date)
            .ToList();
    }

    private async Task<List<Interview>> GetAllUpcomingInterviews(
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var interviews = await _interviewRepository.GetAllAsync(cancellationToken);

        return interviews
            .Where(i => !fromDate.HasValue || i.ScheduledDate >= fromDate.Value.Date)
            .Where(i => !toDate.HasValue || i.ScheduledDate <= toDate.Value.Date)
            .ToList();
    }
}
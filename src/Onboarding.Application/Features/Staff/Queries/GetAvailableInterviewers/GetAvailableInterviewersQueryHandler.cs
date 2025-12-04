// Application/Features/Staff/Queries/GetAvailableInterviewers/GetAvailableInterviewersQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Staff.Queries.GetAvailableInterviewers;

public class GetAvailableInterviewersQueryHandler : IRequestHandler<GetAvailableInterviewersQuery, Result<List<InterviewerDto>>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IInterviewRepository _interviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAvailableInterviewersQueryHandler> _logger;

    public GetAvailableInterviewersQueryHandler(
        IStaffRepository staffRepository,
        IInterviewRepository interviewRepository,
        IUserRepository userRepository,
        ILogger<GetAvailableInterviewersQueryHandler> logger)
    {
        _staffRepository = staffRepository;
        _interviewRepository = interviewRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<InterviewerDto>>> Handle(
        GetAvailableInterviewersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate requesting user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<List<InterviewerDto>>.Failure("User not found");

            // 2. Authorization check - only admins/admission officers can view interviewers
            var isAuthorized = request.UserRole == UserRole.SuperAdmin ||
                              request.UserRole == UserRole.SchoolAdmin ||
                              request.UserRole == UserRole.AdmissionOfficer;

            if (!isAuthorized)
                return Result<List<InterviewerDto>>.Failure("Unauthorized to view interviewers");

            // 3. Get all active teaching staff
            var teachingStaff = await _staffRepository.GetByTypeAsync(StaffType.Teaching, cancellationToken);
            var activeStaff = teachingStaff.Where(s => s.Status == StaffStatus.Active).ToList();

            // 4. Check availability for each staff member
            var interviewerDtos = new List<InterviewerDto>();
            var checkDate = request.Date ?? DateTime.UtcNow.Date;

            foreach (var staff in activeStaff)
            {
                // Check if staff has interviews scheduled on the given date
                var scheduledInterviews = await _interviewRepository.GetByInterviewerIdAsync(staff.Id, cancellationToken);
                var interviewsOnDate = scheduledInterviews
                    .Where(i => i.ScheduledDate.Date == checkDate && i.Status == InterviewStatus.Scheduled)
                    .ToList();

                // Staff is available if they have less than 4 interviews on that day
                var isAvailable = interviewsOnDate.Count < 4;
                var nextAvailableDate = !isAvailable ?
                    await GetNextAvailableDate(staff.Id, checkDate, cancellationToken) :
                    (DateTime?)null;

                var dto = new InterviewerDto(
                    staff.Id,
                    $"{staff.FirstName} {staff.LastName}",
                    staff.Department?.ToString(),
                    staff.Qualification,
                    isAvailable,
                    nextAvailableDate
                );

                interviewerDtos.Add(dto);
            }

            // Sort by availability, then by name
            interviewerDtos = interviewerDtos
                .OrderByDescending(i => i.IsAvailable)
                .ThenBy(i => i.FullName)
                .ToList();

            return Result<List<InterviewerDto>>.Success(interviewerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available interviewers for user {UserId}", request.UserId);
            return Result<List<InterviewerDto>>.Failure("An error occurred while fetching interviewers");
        }
    }

    private async Task<DateTime> GetNextAvailableDate(Guid staffId, DateTime startDate, CancellationToken cancellationToken)
    {
        // Look ahead up to 7 days for availability
        for (int i = 1; i <= 7; i++)
        {
            var checkDate = startDate.AddDays(i);
            var interviews = await _interviewRepository.GetByInterviewerIdAsync(staffId, cancellationToken);
            var interviewsOnDate = interviews
                .Where(i => i.ScheduledDate.Date == checkDate && i.Status == InterviewStatus.Scheduled)
                .ToList();

            if (interviewsOnDate.Count < 4)
                return checkDate;
        }

        // If no availability found in 7 days, return 8 days from start
        return startDate.AddDays(8);
    }
}
// Application/Features/Users/Queries/GetUserProfile/GetUserProfileQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Application.Features.Users.Queries.GetUserProfile;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;

    public GetUserProfileQueryHandler(
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        ILogger<GetUserProfileQueryHandler> logger)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _logger = logger;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get user
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<UserProfileDto>.Failure("User not found");

            // 2. Get user's students if parent
            var studentProfiles = new List<StudentProfileDto>();

            if (user.Role == UserRole.Parent)
            {
                var students = await _studentRepository.GetAllAsync(cancellationToken);
                var userStudents = students.Where(s => s.ParentUserId == user.Id).ToList();

                studentProfiles = userStudents.Select(s => new StudentProfileDto(
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.MiddleName,
                    $"{s.FirstName} {s.LastName}",
                    s.DateOfBirth,
                    CalculateAge(s.DateOfBirth),
                    s.Status,
                    s.AdmissionNumber
                )).ToList();
            }

            // 3. Create response
            var response = new UserProfileDto(
                user.Id,
                user.Email,
                user.PhoneNumber,
                user.FirstName,
                user.LastName,
                user.MiddleName,
                $"{user.FirstName} {user.LastName}",
                user.Role,
                user.Status,
                user.IsEmailVerified,
                user.IsPhoneVerified,
                user.StateOfOrigin,
                user.LocalGovernment,
                user.CreatedAt,
                user.LastLoginAt,
                studentProfiles
            );

            return Result<UserProfileDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for user {UserId}", request.UserId);
            return Result<UserProfileDto>.Failure("An error occurred while fetching profile");
        }
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
            age--;

        return age;
    }
}
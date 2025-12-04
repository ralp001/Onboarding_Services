// Application/Features/Staff/Queries/GetStaffById/GetStaffByIdQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;
using Onboarding.Domain.Entities;

namespace Onboarding.Application.Features.Staff.Queries.GetStaffById;

public class GetStaffByIdQueryHandler : IRequestHandler<GetStaffByIdQuery, Result<StaffDto>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetStaffByIdQueryHandler> _logger;

    public GetStaffByIdQueryHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        ILogger<GetStaffByIdQueryHandler> logger)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<StaffDto>> Handle(
        GetStaffByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get staff
            var staff = await _staffRepository.GetByIdAsync(request.StaffId, cancellationToken);
            if (staff == null)
                return Result<StaffDto>.Failure("Staff not found");

            // 2. Validate requesting user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<StaffDto>.Failure("User not found");

            // 3. Authorization check
            var isAuthorized = CheckAuthorization(request.UserRole, staff);
            if (!isAuthorized)
                return Result<StaffDto>.Failure("Unauthorized to view staff details");

            // 4. Map to DTO
            var staffDto = new StaffDto(
                staff.Id,
                staff.FirstName,
                staff.LastName,
                staff.MiddleName,
                staff.Email,
                staff.PhoneNumber,
                staff.DateOfBirth,
                staff.Gender.ToString(),
                staff.Type,
                staff.Department?.ToString() ?? string.Empty,
                staff.Qualification,
                staff.EmploymentDate,
                staff.Status.ToString(),
                staff.CreatedAt
            );

            return Result<StaffDto>.Success(staffDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff {StaffId}", request.StaffId);
            return Result<StaffDto>.Failure("An error occurred while fetching staff details");
        }
    }

    private bool CheckAuthorization(UserRole userRole, Domain.Entities.Staff staff)
    {
        switch (userRole)
        {
            case UserRole.SuperAdmin:
            case UserRole.SchoolAdmin:
                // Admins can view all staff
                return true;

            case UserRole.Teacher:
                // Teachers can view their own profile or other teaching staff
                return staff.Id == Guid.Parse("user-id-from-token") ||
                       staff.Type == StaffType.Teaching;

            case UserRole.AdmissionOfficer:
                // Admission officers can view teaching staff for interview assignments
                return staff.Type == StaffType.Teaching;

            default:
                return false;
        }
    }
}
// Application/Features/Staff/Commands/CreateStaff/CreateStaffCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Staff.Commands.CreateStaff;

public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Result<Guid>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateStaffCommandHandler> _logger;

    public CreateStaffCommandHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        ILogger<CreateStaffCommandHandler> logger)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateStaffCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate admin user creating staff
            var adminUser = await _userRepository.GetByIdAsync(request.CreatedByUserId, cancellationToken);
            if (adminUser == null)
                return Result<Guid>.Failure("Admin user not found");

            var isAuthorized = adminUser.Role == UserRole.SuperAdmin ||
                              adminUser.Role == UserRole.SchoolAdmin;

            if (!isAuthorized)
                return Result<Guid>.Failure("Only school administrators can create staff");

            // 2. Check if staff email already exists
            var existingStaff = await _staffRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingStaff != null)
                return Result<Guid>.Failure("Staff with this email already exists");

            // 3. Create staff record
            var staff = new Domain.Entities.Staff
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Type = request.Type,
                Department = Enum.Parse<Department>(request.Department),
                Qualification = request.Qualification,
                EmploymentDate = request.EmploymentDate,
                Status = StaffStatus.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedByUserId
            };

            // 4. Save staff
            await _staffRepository.AddAsync(staff, cancellationToken);

            // 5. Create user account for staff if they need system access
            // TODO: Auto-create user account with appropriate role

            _logger.LogInformation(
                "Staff {StaffId} created by admin {AdminId}",
                staff.Id, request.CreatedByUserId);

            return Result<Guid>.Success(staff.Id, "Staff created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff with email {Email}", request.Email);
            return Result<Guid>.Failure("An error occurred while creating staff");
        }
    }
}
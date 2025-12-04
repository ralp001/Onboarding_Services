// Application/Features/Students/Commands/UpdateStudent/UpdateStudentCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;
using Onboarding.Domain.ValueObjects;

namespace Onboarding.Application.Features.Students.Commands.UpdateStudent;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateStudentCommandHandler> _logger;

    public UpdateStudentCommandHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ILogger<UpdateStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verify parent user exists and is parent
            var parentUser = await _userRepository.GetByIdAsync(request.ParentUserId, cancellationToken);
            if (parentUser == null)
                return Result.Failure("Parent user not found");

            if (parentUser.Role != UserRole.Parent)
                return Result.Failure("Only parents can update student information");

            // 2. Get student
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return Result.Failure("Student not found");

            // 3. Verify parent owns this student
            if (student.ParentUserId != request.ParentUserId)
                return Result.Failure("You can only update your own student records");

            // 4. Apply updates if provided
            if (!string.IsNullOrWhiteSpace(request.FirstName))
                student.FirstName = request.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(request.LastName))
                student.LastName = request.LastName.Trim();

            if (request.MiddleName != null)
                student.MiddleName = request.MiddleName.Trim();

            if (request.DateOfBirth.HasValue)
                student.DateOfBirth = request.DateOfBirth.Value;

            if (!string.IsNullOrWhiteSpace(request.Gender))
                student.Gender = request.Gender.Trim();

            if (request.Religion != null)
                student.Religion = request.Religion.Trim();

            // 5. Update contact info if provided
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExists = await _studentRepository.ExistsByEmailAsync(request.Email, cancellationToken);
                if (emailExists && student.Email != request.Email)
                    return Result.Failure("Email is already registered for another student");

                student.Email = request.Email.Trim().ToLower();
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneExists = await _studentRepository.ExistsByPhoneAsync(request.PhoneNumber, cancellationToken);
                if (phoneExists && student.PhoneNumber != request.PhoneNumber)
                    return Result.Failure("Phone number is already registered for another student");

                student.PhoneNumber = request.PhoneNumber.Trim();
            }

            // 6. Update address if any address field is provided
            if (request.AddressStreet != null || request.AddressCity != null ||
                request.AddressLGA != null || request.AddressState.HasValue ||
                request.AddressPostalCode != null)
            {
                var currentAddress = student.Address;

                var newAddress = new NigerianAddress(
                    request.AddressStreet ?? currentAddress.Street,
                    request.AddressCity ?? currentAddress.City,
                    request.AddressLGA ?? currentAddress.LGA,
                    request.AddressState ?? currentAddress.State,
                    request.AddressPostalCode ?? currentAddress.PostalCode);

                student.Address = newAddress;
            }

            // 7. Update academic info
            if (request.PreviousSchool != null)
                student.PreviousSchool = request.PreviousSchool.Trim();

            if (request.PreviousClass != null)
                student.PreviousClass = request.PreviousClass.Trim();

            if (request.StateOfOrigin != null)
                student.StateOfOrigin = request.StateOfOrigin.Trim();

            if (request.LocalGovernment != null)
                student.LocalGovernment = request.LocalGovernment.Trim();

            if (request.Nationality != null)
                student.Nationality = request.Nationality.Trim();

            if (request.SelectedStream.HasValue)
                student.SelectedStream = request.SelectedStream.Value;

            // 8. Save changes
            await _studentRepository.UpdateAsync(student, cancellationToken);

            _logger.LogInformation("Student {StudentId} updated by Parent {ParentUserId}",
                student.Id, request.ParentUserId);

            return Result.Success("Student information updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student {StudentId}", request.StudentId);
            return Result.Failure("An error occurred while updating student information");
        }
    }
}
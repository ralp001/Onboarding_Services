// Application/Features/Students/Commands/CreateStudent/CreateStudentCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;
using Onboarding.Domain.ValueObjects;

namespace Onboarding.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<Guid>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateStudentCommandHandler> _logger;

    public CreateStudentCommandHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ILogger<CreateStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verify Parent User exists and is a Parent role
            var parentUser = await _userRepository.GetByIdAsync(request.ParentUserId, cancellationToken);
            if (parentUser == null)
                return Result<Guid>.Failure("Parent user not found");

            if (parentUser.Role != UserRole.Parent)
                return Result<Guid>.Failure("User must be a parent to create student records");

            if (parentUser.Status != UserStatus.Active)
                return Result<Guid>.Failure("Parent account is not active");

            // 2. Check if student already exists with same email/phone
            var existingStudent = await _studentRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (existingStudent)
                return Result<Guid>.Failure("A student with this email already exists");

            existingStudent = await _studentRepository.ExistsByPhoneAsync(request.PhoneNumber, cancellationToken);
            if (existingStudent)
                return Result<Guid>.Failure("A student with this phone number already exists");

            // 3. Calculate age for Nigerian school eligibility
            var age = CalculateAge(request.DateOfBirth);
            if (age < 10)
                return Result<Guid>.Failure("Student must be at least 10 years old for admission");

            if (age > 18)
                _logger.LogWarning("Student age {Age} is above typical secondary school age", age);

            // 4. Create Nigerian Address Value Object
            var address = new NigerianAddress(
                request.AddressStreet,
                request.AddressCity,
                request.AddressLGA,
                request.AddressState,
                request.AddressPostalCode);

            // 5. Create Student Entity (linked to Parent User)
            var student = new Student
            {
                Id = Guid.NewGuid(),
                ParentUserId = request.ParentUserId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                MiddleName = request.MiddleName?.Trim(),
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender.Trim(),
                Religion = request.Religion?.Trim(),
                Email = request.Email.Trim().ToLower(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = address,
                PreviousSchool = request.PreviousSchool?.Trim(),
                PreviousClass = request.PreviousClass?.Trim(),
                StateOfOrigin = request.StateOfOrigin?.Trim(),
                LocalGovernment = request.LocalGovernment?.Trim(),
                Nationality = request.Nationality?.Trim() ?? "Nigerian",
                SelectedStream = request.PreferredStream,
                Status = StudentStatus.Prospective,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.ParentUserId
            };

            // 6. Save to database
            await _studentRepository.AddAsync(student, cancellationToken);

            _logger.LogInformation(
                "Student {StudentId} created by Parent {ParentUserId}",
                student.Id, request.ParentUserId);

            return Result<Guid>.Success(student.Id, "Student created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student for Parent {ParentUserId}", request.ParentUserId);
            return Result<Guid>.Failure("An error occurred while creating student");
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
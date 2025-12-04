// Application/Features/Students/DTOs/StudentDto.cs
namespace Onboarding.Application.DTOs;

public class StudentDto
{
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }
    public DateTime DateOfBirth { get; }
    public string Gender { get; }
    public string? Nationality { get; }
    public string? StateOfOrigin { get; }
    public string? LocalGovernment { get; }
    public string? Religion { get; }
    public string MedicalConditions { get; }
    public string? PreviousSchool { get; }
    public string? PreviousClass { get; }
    public string Type { get; }
    public string Status { get; }
    public DateTime CreatedAt { get; }

    public StudentDto(
        Guid id,
        string firstName,
        string lastName,
        string? middleName,
        DateTime dateOfBirth,
        string gender,
        string? nationality,
        string? stateOfOrigin,
        string? localGovernment,
        string? religion,
        string medicalConditions,
        string? previousSchool,
        string? previousClass,
        string type,
        string status,
        DateTime createdAt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Nationality = nationality;
        StateOfOrigin = stateOfOrigin;
        LocalGovernment = localGovernment;
        Religion = religion;
        MedicalConditions = medicalConditions;
        PreviousSchool = previousSchool;
        PreviousClass = previousClass;
        Type = type;
        Status = status;
        CreatedAt = createdAt;
    }
}
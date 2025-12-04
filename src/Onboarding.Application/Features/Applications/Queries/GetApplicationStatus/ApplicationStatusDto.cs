// Application/Features/Applications/Queries/GetApplicationStatus/ApplicationStatusDto.cs
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Applications.Queries.GetApplicationStatus;

public record ApplicationStatusDto(
    Guid ApplicationId,
    string ApplicationNumber,
    string StudentName,
    ApplicationStatus Status,
    DateTime AppliedDate,
    DateTime? DecisionDate,
    string? DecisionRemarks,
    List<ApplicationStageDto> Stages,
    List<DocumentStatusDto> RequiredDocuments,
    InterviewDetailsDto? InterviewDetails);

public record ApplicationStageDto(
    string StageName,
    string Status,
    DateTime? CompletedDate,
    string? Remarks);

public record DocumentStatusDto(
    DocumentType Type,
    string Name,
    bool IsUploaded,
    bool IsVerified,
    DateTime? UploadDate);

public record InterviewDetailsDto(
    DateTime ScheduledDate,
    TimeSpan ScheduledTime,
    InterviewType Type,
    InterviewStatus Status,
    string? MeetingLink,
    string? InterviewerName);
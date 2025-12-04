// Application/Features/Staff/Commands/UpdateStaffStatus/UpdateStaffStatusCommandValidator.cs
using FluentValidation;

namespace Onboarding.Application.Features.Staff.Commands.UpdateStaffStatus;

public class UpdateStaffStatusCommandValidator : AbstractValidator<UpdateStaffStatusCommand>
{
    public UpdateStaffStatusCommandValidator()
    {
        RuleFor(x => x.StaffId)
            .NotEmpty().WithMessage("Staff ID is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid staff status");

        RuleFor(x => x.UpdatedByUserId)
            .NotEmpty().WithMessage("Updated by user ID is required");
    }
}
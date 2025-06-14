using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.Validators;

/// <summary>
/// Validator for the UnlockAchievementCommand.
/// </summary>
public class UnlockAchievementCmdVal : AbstractValidator<UnlockAchievementCmd>
{
    public UnlockAchievementCmdVal()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required");

        RuleFor(x => x.AchievementId)
            .NotEmpty()
            .WithMessage("AchievementId is required");

        RuleFor(x => x.UnlockedAtUtc)
            .NotEmpty()
            .WithMessage("UnlockedAtUtc is required")
            .Must((context, unlockedAtUtc) => unlockedAtUtc <= DateTime.UtcNow)
            .WithMessage("UnlockedAtUtc must be in the past or present");
    }
}

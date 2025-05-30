using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.Validators;

/// <summary>
/// Validator for the UnlockAchievementCommand.
/// </summary>
public class UnlockAchievementCommandValidator : AbstractGrainValidator<UnlockAchievementCommand, PlayerState, Guid>
{
    public UnlockAchievementCommandValidator()
    {
        RuleFor(x => x.Command.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required")
            .Must((context, playerId) => playerId == context.GrainPrimaryKey)
            .WithMessage("PlayerId must match grain primary key");

        RuleFor(x => x.Command.AchievementId)
            .NotEmpty()
            .WithMessage("AchievementId is required")
            .Must((context, achievementId) => !context.GrainState.UnlockedAchievements.ContainsKey(achievementId))
            .WithMessage("AchievementId must not already be unlocked by the player");

        RuleFor(x => x.Command.UnlockedAtUtc)
            .NotEmpty()
            .WithMessage("UnlockedAtUtc is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("UnlockedAtUtc must be in the past or present");
    }
}

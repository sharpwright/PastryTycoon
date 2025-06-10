using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game.Validators;

/// <summary>
/// Validator for the UpdateGameCommand.
/// </summary>
public class UpdateGameCmdVal : AbstractValidator<UpdateGameCmd>
{
    public UpdateGameCmdVal()
    {
        RuleFor(x => x.GameId)
            .NotEmpty()
            .WithMessage("GameId is required");

        RuleFor(x => x.UpdateTimeUtc)
            .NotEmpty()
            .WithMessage("UpdateTimeUtc is required")
            .Must(updateTime => updateTime <= DateTime.UtcNow)
            .WithMessage("UpdateTimeUtc must be in the past or present");
    }
}

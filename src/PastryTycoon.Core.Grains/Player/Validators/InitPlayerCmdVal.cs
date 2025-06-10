using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.Validators;

/// <summary>
/// Validator for the InitializePlayerCommand.
/// </summary>
public class InitPlayerCmdVal : AbstractValidator<InitPlayerCmd>
{
    public InitPlayerCmdVal()
    {
        RuleFor(x => x.PlayerName)
            .NotEmpty()
            .WithMessage("PlayerName is required")
            .MaximumLength(50)
            .WithMessage("PlayerName cannot exceed 50 characters");

        RuleFor(x => x.GameId)
            .NotEmpty()
            .WithMessage("GameId is required");
    }
}

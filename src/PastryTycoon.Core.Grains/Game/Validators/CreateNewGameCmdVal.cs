using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game.Validators;

/// <summary>
/// Validator for the CreateNewGameCommand.
/// </summary>
public class CreateNewGameCmdVal : AbstractValidator<CreateNewGameCmd>
{
    public CreateNewGameCmdVal()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required");

        RuleFor(x => x.PlayerName)
            .NotEmpty()
            .WithMessage("PlayerName is required");

        RuleFor(x => x.DifficultyLevel)
            .IsInEnum()
            .WithMessage("DifficultyLevel is invalid");
    }
}

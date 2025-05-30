using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game.Validators;

public class CreateNewGameCommandValidator : AbstractGrainValidator<CreateNewGameCommand, object, Guid>
{
    public CreateNewGameCommandValidator()
    {
        RuleFor(x => x.Command.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required");

        RuleFor(x => x.Command.PlayerName)
            .NotEmpty()
            .WithMessage("PlayerName is required");

        RuleFor(x => x.Command.DifficultyLevel)
            .IsInEnum()
            .WithMessage("DifficultyLevel is invalid");
    }
}

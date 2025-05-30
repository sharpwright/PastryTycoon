using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game.Validators;

/// <summary>
/// Validator for the UpdateGameCommand.
/// </summary>
public class UpdateGameCommandValidator : AbstractGrainValidator<UpdateGameCommand, GameState, Guid>
{
    public UpdateGameCommandValidator()
    {
        RuleFor(x => x.Command.GameId)
            .NotEmpty()
            .WithMessage("GameId is required")
            .Must((context, gameId) => gameId == context.GrainPrimaryKey)
            .WithMessage("GameId must match grain primary key");

        RuleFor(x => x.Command.GameName)
            .NotEmpty()
            .WithMessage("GameName is required");

        RuleFor(x => x.Command.UpdateTimeUtc)
            .NotEmpty()
            .WithMessage("UpdateTimeUtc is required")
            .Must(updateTime => updateTime <= DateTime.UtcNow)
            .WithMessage("UpdateTimeUtc must be in the past or present");
    }
}

using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game.CommandHandlers;

public class UpdateGameCmdHdlr : ICommandHandler<UpdateGameCmd, GameState, GameEvent>
{
    private readonly IValidator<UpdateGameCmd> validator;

    public UpdateGameCmdHdlr(IValidator<UpdateGameCmd> validator)
    {
        this.validator = validator;
    }

    public async Task<CommandHandlerResult<GameEvent>> HandleAsync(UpdateGameCmd command, GameState state, string grainId)
    {
        // Validate the command.
        var results = await validator.ValidateAsync(command);
        if (!results.IsValid)
        {
            return CommandHandlerResult<GameEvent>.Failure([.. results.Errors.Select(e => e.ErrorMessage)]);
        }

        if (!state.IsInitialized)
        {
            // If the game state is not initialized, return a failure.
            return CommandHandlerResult<GameEvent>.Failure("Game state is not initialized.");
        }

        // Create the event to update the game state.
        var evt = new GameUpdatedEvent(
            command.GameId,
            command.UpdateTimeUtc);

        // Return the result with the event.
        return CommandHandlerResult<GameEvent>.Success(evt);
    }

}

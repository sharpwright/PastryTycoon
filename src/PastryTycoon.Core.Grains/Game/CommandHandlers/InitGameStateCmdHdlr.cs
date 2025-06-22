using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game.CommandHandlers;

public class InitGameStateCmdHdlr : ICommandHandler<InitGameStateCmd, GameState, GameEvent>
{
    private readonly IValidator<InitGameStateCmd> validator;

    public InitGameStateCmdHdlr(IValidator<InitGameStateCmd> validator)
    {
        this.validator = validator;
    }

    public async Task<CommandHandlerResult<GameEvent>> HandleAsync(InitGameStateCmd command, GameState state, string grainId)
    {
        // Validate the command.
        var results = await validator.ValidateAsync(command);
        if (!results.IsValid)
        {
            return CommandHandlerResult<GameEvent>.Failure([.. results.Errors.Select(e => e.ErrorMessage)]);
        }

        if (state.IsInitialized)
        {
            // If the game state is already initialized, return a failure.
            return CommandHandlerResult<GameEvent>.Failure("Game state is already initialized.");
        }

        // Create the event to initialize the game state.
        var evt = new GameStateInitializedEvent(
            command.GameId.ToString("N"),
            command.PlayerId.ToString("N"),
            command.RecipeIds,
            command.StartTimeUtc);

        // Return the result with the event.
        return CommandHandlerResult<GameEvent>.Success(evt);
    }
}

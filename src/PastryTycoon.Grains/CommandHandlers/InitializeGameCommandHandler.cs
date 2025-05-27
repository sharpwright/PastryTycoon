using System;
using Microsoft.Extensions.Logging;
using PastryTycoon.Common.Commands;
using PastryTycoon.Grains.Events;
using PastryTycoon.Grains.States;
using PastryTycoon.Grains.Validation;

namespace PastryTycoon.Grains.CommandHandlers;

public class InitializeGameCommandHandler : ICommandHandler<InitializeGameStateCommand, GameState, Guid>
{
    private readonly ILogger<InitializeGameCommandHandler> logger;
    private readonly InitializeGameStateCommandValidator validator;

    public InitializeGameCommandHandler(ILogger<InitializeGameCommandHandler> logger, InitializeGameStateCommandValidator validator)
    {
        this.logger = logger;
        this.validator = validator;
    }

    public async Task<CommandResult> HandleAsync(InitializeGameStateCommand command, GameState state, Guid grainKey)
    {
        try 
        {
            await validator.ValidateCommandAsync(command, state, grainKey);

            if (state.IsInitialized)
            {
                return CommandResult.Failure("Game is already initialized");
            }

            var evt = new GameStateInitializedEvent(
                command.GameId, 
                command.PlayerId, 
                command.RecipeIds, 
                command.GameName, 
                command.StartTimeUtc);

            return CommandResult.Success(evt);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle InitializeGameStateCommand for GameId: {GameId}", command.GameId);
            return CommandResult.Failure(ex.Message);
        }
    }
}

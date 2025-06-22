using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.CommandHandlers;

public class InitPlayerCmdHdlr : ICommandHandler<InitPlayerCmd, PlayerState, PlayerEvent>
{
    private readonly IValidator<InitPlayerCmd> validator;

    public InitPlayerCmdHdlr(IValidator<InitPlayerCmd> validator)
    {
        this.validator = validator;
    }

    public async Task<CommandHandlerResult<PlayerEvent>> HandleAsync(InitPlayerCmd command, PlayerState state, string grainId)
    {
        // Validate the command
        var results = await validator.ValidateAsync(command);

        if (!results.IsValid)
        {
            // If validation fails, return a failure result with errors
            return CommandHandlerResult<PlayerEvent>.Failure([.. results.Errors.Select(e => e.ErrorMessage)]);
        }

        if (state.IsInitialized)
        {
            // If the player is already initialized, return a failure result
            return CommandHandlerResult<PlayerEvent>.Failure("Player is already initialized.");
        }

        // Initialize the player state with the provided command data
        return CommandHandlerResult<PlayerEvent>.Success(
            new PlayerInitializedEvent(
                grainId,
                command.PlayerName,
                command.GameId.ToString("N"),
                DateTime.UtcNow));
    }
}

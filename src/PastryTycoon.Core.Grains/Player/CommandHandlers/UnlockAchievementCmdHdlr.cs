using System;
using System.Windows.Input;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.CommandHandlers;

public class UnlockAchievementCmdHdlr : ICommandHandler<UnlockAchievementCmd, PlayerState, PlayerEvent>
{
    private readonly IValidator<UnlockAchievementCmd> validator;

    public UnlockAchievementCmdHdlr(IValidator<UnlockAchievementCmd> validator)
    {
        this.validator = validator;
    }

    public async Task<CommandHandlerResult<PlayerEvent>> HandleAsync(UnlockAchievementCmd command, PlayerState state, string grainId)
    {
        var results = await validator.ValidateAsync(command);

        if (!results.IsValid)
        {
            // If validation fails, return a failure result with errors
            return CommandHandlerResult<PlayerEvent>.Failure([.. results.Errors.Select(e => e.ErrorMessage)]);
        }

        // Check if the player validity
        if (!state.IsInitialized
            || grainId != command.PlayerId.ToString("N")
            || state.PlayerId != command.PlayerId.ToString("N"))
        {
            return CommandHandlerResult<PlayerEvent>.Failure("Invalid player state for unlocking achievement.");
        }

        // Check if the achievement is already unlocked
        if (state.UnlockedAchievements.ContainsKey(command.AchievementId))
        {
            return CommandHandlerResult<PlayerEvent>.Failure("Achievement is already unlocked.");
        }

        // Create an event for unlocking the achievement
        var achievementUnlockedEvent = new PlayerUnlockedAchievementEvent(
            command.PlayerId.ToString("N"),
            command.AchievementId,
            DateTime.UtcNow);

        return CommandHandlerResult<PlayerEvent>.Success(achievementUnlockedEvent);
    }
}
using System;
using PastryTycoon.Core.Abstractions.Constants;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;
using System.IO.Pipelines;
using PastryTycoon.Core.Abstractions.Common;

namespace PastryTycoon.Core.Grains.Player;

/// <summary>
/// Grain representing a player in the game.
/// </summary>
[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS)]
public class PlayerGrain : JournaledGrain<PlayerState, PlayerEvent>, IPlayerGrain
{
    private IAsyncStream<PlayerEvent>? playerEventStream;    
    private readonly ICommandHandler<InitPlayerCmd, PlayerState, PlayerEvent> initializePlayerHandler;
    private readonly ICommandHandler<TryDiscoverRecipeCmd, PlayerState, PlayerEvent> recipeDiscoveryHandler;
    private readonly ICommandHandler<UnlockAchievementCmd, PlayerState, PlayerEvent> unlockAchievementHandler;

    public PlayerGrain(
        ICommandHandler<InitPlayerCmd, PlayerState, PlayerEvent> initializePlayerHandler,
        ICommandHandler<TryDiscoverRecipeCmd, PlayerState, PlayerEvent> recipeDiscoveryHandler,
        ICommandHandler<UnlockAchievementCmd, PlayerState, PlayerEvent> unlockAchievementHandler)
    {
        this.initializePlayerHandler = initializePlayerHandler;
        this.recipeDiscoveryHandler = recipeDiscoveryHandler;
        this.unlockAchievementHandler = unlockAchievementHandler;
    }

    /// <summary>
    /// Called when the grain is activated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for activation.</param>
    /// <returns></returns>
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        playerEventStream = streamProvider.GetStream<PlayerEvent>(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, this.GetPrimaryKeyString());
        await base.OnActivateAsync(cancellationToken);
    }

    /// <summary>
    /// Initializes the player state based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the player initialization details.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the player is already initialized.</exception>
    public async Task<CommandResult> InitializeAsync(InitPlayerCmd command)
    {
        var handlerResult = await initializePlayerHandler.HandleAsync(command, State, this.GetPrimaryKeyString());

        if (!handlerResult.IsSuccess)
        {
            // If the handler result indicates failure, return the errors
            return CommandResult.Failure([.. handlerResult.Errors]);
        }

        if (handlerResult.IsSuccess && handlerResult.Event != default)
        {
            RaiseEvent(handlerResult.Event);
            await ConfirmEvents();
        }

        return CommandResult.Success();        
    }

    /// <summary>
    /// Discovers a recipe for the player based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the recipe discovery details.</param>
    /// <returns></returns>
    public async Task<CommandResult> TryDiscoverRecipeFromIngredientsAsync(TryDiscoverRecipeCmd command)
    {
        // Use the handler to process the command and get a potential event
        var handlerResult = await recipeDiscoveryHandler.HandleAsync(command, State, this.GetPrimaryKeyString());

        if (!handlerResult.IsSuccess)
        {
            // If the handler result indicates failure, return the errors
            return CommandResult.Failure([.. handlerResult.Errors]);
        }

        // If the handler result indicates success, proceed to raise events if any were produced.
        if (handlerResult.IsSuccess && handlerResult.Event != default)
        {
            RaiseEvent(handlerResult.Event);
            await ConfirmEvents();

            if (playerEventStream != null &&
                handlerResult.Event is PlayerDiscoveredRecipeEvent)
            {
                // If the event stream is available, send the discovered recipe event
                await playerEventStream.OnNextAsync(handlerResult.Event);
            }
        }
        
        return CommandResult.Success();
    }

    /// <summary>
    /// Unlocks an achievement for the player based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the achievement unlock details.</param>
    /// <returns></returns>
    public virtual async Task<CommandResult> UnlockAchievementAsync(UnlockAchievementCmd command)
    {
        var handlerResult = await unlockAchievementHandler.HandleAsync(command, State, this.GetPrimaryKeyString());

        if (!handlerResult.IsSuccess)
        {
            // If the handler result indicates failure, return the errors
            return CommandResult.Failure([.. handlerResult.Errors]);
        }

        if (handlerResult.IsSuccess || handlerResult.Event != default)
        {
            RaiseEvent(handlerResult.Event);
            await ConfirmEvents();
        }

        return CommandResult.Success();
    }

    /// <summary>
    /// Retrieves the player statistics asynchronously.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the player is not initialized.</exception>
    public Task<PlayerStatisticsDto> GetPlayerStatisticsAsync()
    {
        if (!State.IsInitialized)
        {
            throw new InvalidOperationException("Player is not initialized.");
        }

        return Task.FromResult(new PlayerStatisticsDto(
            this.GetPrimaryKey(),
            State.PlayerName,
            State.UnlockedAchievements.Count,
            State.CraftedRecipes.Count,
            State.DiscoveredRecipes.Count,
            State.CreatedAtUtc,
            State.LastActivityAtUtc));
    }
}

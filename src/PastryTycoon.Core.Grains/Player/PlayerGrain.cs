using System;
using PastryTycoon.Core.Abstractions.Constants;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Data.Recipes;
using PastryTycoon.Core.Grains.Player.Validators;
using PastryTycoon.Core.Grains.Player.CommandHandlers;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player;

/// <summary>
/// Grain representing a player in the game.
/// </summary>
[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS)]
public class PlayerGrain : JournaledGrain<PlayerState, PlayerEvent>, IPlayerGrain
{
    private IAsyncStream<PlayerEvent>? playerEventStream;
    private readonly IGrainValidator<InitializePlayerCommand, PlayerState, Guid> initializeValidator;
    private readonly IGrainValidator<UnlockAchievementCommand, PlayerState, Guid> unlockAchievementValidator;
    private readonly ICommandHandler<TryDiscoverRecipeCommand, PlayerEvent, PlayerState> recipeDiscoveryHandler;

    public PlayerGrain(
        IGrainValidator<InitializePlayerCommand, PlayerState, Guid> initializeValidator,
        IGrainValidator<UnlockAchievementCommand, PlayerState, Guid> unlockAchievementValidator,
        ICommandHandler<TryDiscoverRecipeCommand, PlayerEvent, PlayerState> recipeDiscoveryHandler)
    {
        this.initializeValidator = initializeValidator;
        this.unlockAchievementValidator = unlockAchievementValidator;
        this.recipeDiscoveryHandler = recipeDiscoveryHandler;
    }

    /// <summary>
    /// Called when the grain is activated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for activation.</param>
    /// <returns></returns>
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        playerEventStream = streamProvider.GetStream<PlayerEvent>(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, this.GetPrimaryKey());
        await base.OnActivateAsync(cancellationToken);
    }

    /// <summary>
    /// Initializes the player state based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the player initialization details.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the player is already initialized.</exception>
    public async Task InitializeAsync(InitializePlayerCommand command)
    {
        await initializeValidator.ValidateCommandAndThrowsAsync(command, State, this.GetPrimaryKey());        

        var evt = new PlayerInitializedEvent(
            this.GetPrimaryKey(),
            command.PlayerName,
            command.GameId,
            DateTime.UtcNow);

        RaiseEvent(evt);
        await ConfirmEvents();
    }

    /// <summary>
    /// Discovers a recipe for the player based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the recipe discovery details.</param>
    /// <returns></returns>
    public async Task TryDiscoverRecipeFromIngredientsAsync(TryDiscoverRecipeCommand command)
    {
        // Use the handler to process the command and get a potential event
        var evt = await recipeDiscoveryHandler.HandleAsync(command, State, this.GetPrimaryKey());

        // If the handler returned an event, raise it
        if (evt != null)
        {
            RaiseEvent(evt);
            await ConfirmEvents();

            // Publish to stream if available
            if (playerEventStream != null)
            {
                await playerEventStream.OnNextAsync(evt);
            }
        }
    }

    /// <summary>
    /// Unlocks an achievement for the player based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the achievement unlock details.</param>
    /// <returns></returns>
    public virtual async Task UnlockAchievementAsync(UnlockAchievementCommand command)
    {
        await unlockAchievementValidator.ValidateCommandAndThrowsAsync(command, State, this.GetPrimaryKey());

        var achievementId = command.AchievementId;
        var unlockedAtUtc = command.UnlockedAtUtc;

        // Update player state to include the new achievement
        var evt = new PlayerUnlockedAchievementEvent(
            command.PlayerId,
            achievementId,
            unlockedAtUtc);

        RaiseEvent(evt);
        await ConfirmEvents();
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

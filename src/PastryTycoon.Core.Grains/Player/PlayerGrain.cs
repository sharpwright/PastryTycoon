using System;
using PastryTycoon.Core.Abstractions.Constants;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Data.Recipes;
using PastryTycoon.Core.Grains.Player.Validators;

namespace PastryTycoon.Core.Grains.Player;

[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS)]
public class PlayerGrain : JournaledGrain<PlayerState, PlayerEvent>, IPlayerGrain
{
    private IAsyncStream<PlayerEvent>? playerEventStream;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        playerEventStream = streamProvider.GetStream<PlayerEvent>(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, this.GetPrimaryKey());
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task InitializeAsync(InitializePlayerCommand command)
    {
        var validator = new InitializePlayerCommandValidator();
        await validator.ValidateCommandAndThrowsAsync(command, State, this.GetPrimaryKey());

        if (State.IsInitialized)
        {
            throw new InvalidOperationException("Player is already initialized.");
        }

        var evt = new PlayerInitializedEvent
        {
            PlayerId = this.GetPrimaryKey(),
            PlayerName = command.PlayerName,
            GameId = command.GameId,
            CreatedAtUtc = DateTime.UtcNow
        };

        RaiseEvent(evt);
        await ConfirmEvents();
    }

    public async Task DiscoverRecipeAsync(DiscoverRecipeCommand command)
    {
        var validator = new DiscoverRecipeCommandValidator();
        await validator.ValidateCommandAndThrowsAsync(command, State, this.GetPrimaryKey());

        // If the recipe is not already known, apply the discovery.
        var evt = new PlayerDiscoveredRecipeEvent
        {
            PlayerId = command.PlayerId,
            RecipeId = command.RecipeId,
            DiscoveryTimeUtc = command.DiscoveryTimeUtc
        };
        RaiseEvent(evt);
        await ConfirmEvents();

        if (playerEventStream != null)
        {
            await playerEventStream.OnNextAsync(evt);
        }
    }    

    public virtual async Task UnlockAchievementAsync(UnlockAchievementCommand command)
    {
        var validator = new UnlockAchievementCommandValidator();
        await validator.ValidateCommandAndThrowsAsync(command, State, this.GetPrimaryKey());

        var achievementId = command.AchievementId;
        var unlockedAtUtc = command.UnlockedAtUtc;

        // Update player state to include the new achievement
        var evt = new PlayerUnlockedAchievementEvent
        {
            PlayerId = command.PlayerId,
            AchievementId = achievementId,
            UnlockedAtUtc = unlockedAtUtc
        };
        RaiseEvent(evt);
        await ConfirmEvents();
    }

    public Task<PlayerStatisticsDto> GetPlayerStatisticsAsync()
    {
        if (!State.IsInitialized)
        {
            throw new InvalidOperationException("Player is not initialized.");
        }
        
        return Task.FromResult(new PlayerStatisticsDto
        {
            PlayerId = this.GetPrimaryKey(),
            PlayerName = State.PlayerName,
            TotalRecipesDiscovered = State.DiscoveredRecipeIds.Count,
            TotalAchievementsUnlocked = State.UnlockedAchievements.Count,
            CreatedAtUtc = State.CreatedAtUtc,
            LastActivityAtUtc = State.LastActivityAtUtc
        });
    }
}

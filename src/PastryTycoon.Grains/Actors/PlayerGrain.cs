using System;
using PastryTycoon.Common.Constants;
using PastryTycoon.Common.Commands;
using PastryTycoon.Common.Events;
using PastryTycoon.Common.States;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using PastryTycoon.Common.Actors;

namespace PastryTycoon.Grains.Actors;

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

    public async Task DiscoverRecipe(DiscoverRecipeCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command), "DiscoverRecipeCommand cannot be null.");
        }

        if (!State.KnownRecipeIds.ContainsKey(command.RecipeId))
        {
            // If the recipe is not already known, apply the discovery.
            var evt = new RecipeDiscoveredEvent(command.PlayerId, command.RecipeId, command.DiscoveryTimeUtc);
            RaiseEvent(evt);
            await ConfirmEvents();

            if (playerEventStream != null)
            {
                await playerEventStream.OnNextAsync(evt);
            }
        }        
    }

    public async Task UnlockAchievementAsync(string achievement, DateTime unlockedAtUtc)
    {
        if (string.IsNullOrEmpty(achievement))
        {
            throw new ArgumentException("Achievement cannot be null or empty.", nameof(achievement));
        }

        if (!State.Achievements.ContainsKey(achievement))
        {
            // If the achievement is not already unlocked, apply the achievement unlock.            
            var evt = new AchievementUnlockedEvent(State.PlayerId, achievement, unlockedAtUtc);
            RaiseEvent(evt);
            await ConfirmEvents();
        }
    }
}

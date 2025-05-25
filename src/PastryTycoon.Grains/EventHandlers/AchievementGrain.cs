using System;
using PastryTycoon.Common.Constants;
using PastryTycoon.Common.Events;
using Orleans.Streams;
using Orleans.Streams.Core;
using PastryTycoon.Common.Actors;
using PastryTycoon.Common.EventHandlers;

namespace PastryTycoon.Grains.EventHandlers;

[ImplicitStreamSubscription(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS)]
public class AchievementGrain : Grain, IAchievementGrain,
    IAsyncObserver<PlayerEvent>,
    IStreamSubscriptionObserver
{

    public async Task OnNextAsync(PlayerEvent item, StreamSequenceToken? token = null)
    {
        if (item is RecipeDiscoveredEvent recipeEvent)
        {
            // Call player grain to add unlocked achievement to the player state.
            var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(this.GetPrimaryKey());
            await playerGrain.UnlockAchievementAsync("FirstRecipeDiscovered", DateTime.UtcNow);

            // Push event to stream for other listeners (if needed)
            // TODO: Implement stream logic if necessary
        }
    }

    public Task OnCompletedAsync() => Task.CompletedTask;
    public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        // Plug our IGameStartedEventObserver to the stream
        var handle = handleFactory.Create<PlayerEvent>();
        await handle.ResumeAsync(this);
    }
}

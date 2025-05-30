using System;
using PastryTycoon.Core.Abstractions.Constants;
using Orleans.Streams;
using Orleans.Streams.Core;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Abstractions.Achievements;
using Microsoft.Extensions.Logging;
using PastryTycoon.Core.Grains.Achievements.UnlockHandlers;

namespace PastryTycoon.Core.Grains.Achievements;

[ImplicitStreamSubscription(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS)]
public class AchievementsGrain : Grain, IAchievementsGrain,
    IAsyncObserver<PlayerEvent>,
    IStreamSubscriptionObserver
{
    private readonly ILogger<IAchievementsGrain> logger;
    private readonly IPersistentState<AchievementsState> state;
    
    private readonly IList<IUnlockHandler> unlockHandlers = new List<IUnlockHandler>
    {
        new FirstRecipeDiscoveredUnlockHandler()
        // Add other unlock handlers here as needed
    };

    public AchievementsGrain(ILogger<IAchievementsGrain> logger,
        [PersistentState(OrleansConstants.GRAIN_STATE_ACHIEVEMENTS, OrleansConstants.GRAIN_STATE_ACHIEVEMENTS)] IPersistentState<AchievementsState> state)
    {
        this.logger = logger;
        this.state = state;
    }

    public async Task OnNextAsync(PlayerEvent item, StreamSequenceToken? token = null)
    {
        this.logger.LogInformation("AchievementsGrain received event: {EventType}", item.GetType().Name);

        switch (item)
        {
            case PlayerDiscoveredRecipeEvent e:
                this.state.State.RecipesDiscovered++;
                break;
            default:
                this.logger.LogWarning("Unhandled event type: {EventType}", item.GetType().Name);
                return; // Ignore unhandled events
        }

        // Save the state after processing the event
        await this.state.WriteStateAsync();

        // Handle the event to check for achievements
        await this.HandleEvent(item);        
    }

    private async Task HandleEvent(PlayerEvent item)
    {
        foreach (var handler in this.unlockHandlers)
        {
            try
            {
                var result = await handler.CheckUnlockConditionAsync(item, this.state.State);
                if (result.IsUnlocked && result.AchievementId != null)
                {
                    // Call player grain to add unlocked achievement to the player state.
                    var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(item.PlayerId);
                    var command = new UnlockAchievementCommand(
                        PlayerId: item.PlayerId,
                        AchievementId: result.AchievementId,
                        UnlockedAtUtc: DateTime.UtcNow
                    );
                    await playerGrain.UnlockAchievementAsync(command);
                    this.logger.LogInformation("Achievement unlocked: {AchievementId}", result.AchievementId);                    
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error checking unlock condition for handler: {HandlerType}", handler.GetType().Name);
            }
        }
    }

    public Task OnCompletedAsync() => Task.CompletedTask;
    public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        // Plug our observer to the stream
        var handle = handleFactory.Create<PlayerEvent>();
        await handle.ResumeAsync(this);
    }
}

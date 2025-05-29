using System;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.EventHandlers;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Streams.Core;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Example of a grain using an implicit stream subscription.
/// A subscription is created on the entire namespace and all events are handled by the observer.
/// This grain will be automatically activated when events are published to the stream.
/// NOTE: calling this.GetPrimaryKey(); will return the producers grain's identity.
/// </summary>
[ImplicitStreamSubscription(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS)]
public class GameProjectionGrain : Grain,
    IGameProjectionGrain,
    IAsyncObserver<GameEvent>,
    IStreamSubscriptionObserver
{
    private readonly ILogger<IGameProjectionGrain> logger;

    public GameProjectionGrain(ILogger<IGameProjectionGrain> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Handles the subscription to the stream.
    /// </summary>
    /// <param name="handleFactory"></param>
    /// <returns></returns>
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        // Plug our IGameStartedEventObserver to the stream
        var handle = handleFactory.Create<GameEvent>();
        
        // NOTE: without calling ResumeAsync the stream will 
        // not see any subscribers and will drop the data.
        await handle.ResumeAsync(this);
    }

    public Task OnNextAsync(GameEvent item, StreamSequenceToken? token = null)
    {
        // Handle the GameEvent here
        switch (item)
        {
            case GameStateInitializedEvent started:
                Handle(started, token);
                break;
            case GameUpdatedEvent updated:
                Handle(updated, token);
                break;
            default:
                logger.LogWarning("Unknown event type received.");
                break;
        }
        return Task.CompletedTask;
    }

    public Task Handle(GameStateInitializedEvent item, StreamSequenceToken? token = null)
    {
        // TODO: Handle the GameStartedEvent here.
        logger.LogInformation($"Game started at {item.StartTimeUtc}");
        return Task.CompletedTask;
    }

    public Task Handle(GameUpdatedEvent item, StreamSequenceToken? token = null)
    {
        // TODO: Handle the GameUpdatedEvent here
        logger.LogInformation($"Game updated at {item.UpdateTimeUtc}");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        // TODO: Handle error
        logger.LogError(ex, $"Error: {ex.Message}");
        return Task.CompletedTask;
    }
}
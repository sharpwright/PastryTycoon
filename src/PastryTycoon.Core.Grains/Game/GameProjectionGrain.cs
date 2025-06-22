using System;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Streams.Core;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Abstractions.Common;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Grain that keeps track of game events and updates the game projection accordingly.
/// </summary>
/// <remarks
///     <para>Example of a grain using an implicit stream subscription.
///     <para>
///         A subscription is created on the entire namespace and all events are handled by the observer.
///         This grain will be automatically activated when events are published to the stream.
///     </para> 
///     <para>NOTE: calling this.GetPrimaryKey(); will return the producers grain's identity.</para>
/// </remarks>
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

    /// <summary>
    /// Handles the next game event by processing it based on its type.
    /// </summary>
    /// <param name="item">The game event to process.</param>
    /// <param name="token">The sequence token for the event, if applicable.</param>
    /// <returns></returns>
    public async Task OnNextAsync(GameEvent item, StreamSequenceToken? token = null)
    {
        // Handle the GameEvent here
        switch (item)
        {
            case GameStateInitializedEvent started:
                await HandleGameInitiliazedEventAsync(started, token);
                break;
            case GameUpdatedEvent updated:
                await HandleGameUpdatedEventAsync(updated, token);
                break;
            default:
                logger.LogWarning("Unhandled GameEvent type: {EventType}", item.GetType().Name);
                break;
        }
    }

    /// <summary>
    /// Handles the GameStateInitializedEvent and updates the game projection.
    /// </summary>
    /// <param name="item">The GameStateInitializedEvent to handle.</param>
    /// <param name="token">The sequence token for the event, if applicable.</param>
    /// <returns></returns>
    public Task HandleGameInitiliazedEventAsync(GameStateInitializedEvent item, StreamSequenceToken? token = null)
    {
        // TODO: Handle the GameStartedEvent here.
        logger.LogInformation($"Game started at {item.StartTimeUtc}");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the GameUpdatedEvent and updates the game projection.
    /// </summary>
    /// <param name="item">The GameUpdatedEvent to handle.</param>
    /// <param name="token">The sequence token for the event, if applicable.</param>
    /// <returns></returns>
    public Task HandleGameUpdatedEventAsync(GameUpdatedEvent item, StreamSequenceToken? token = null)
    {
        // TODO: Handle the GameUpdatedEvent here
        logger.LogInformation($"Game updated at {item.UpdateTimeUtc}");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles errors that occur during stream processing.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <returns></returns>
    public Task OnErrorAsync(Exception ex)
    {
        // TODO: Handle error
        logger.LogError(ex, $"Error: {ex.Message}");
        return Task.CompletedTask;
    }
}
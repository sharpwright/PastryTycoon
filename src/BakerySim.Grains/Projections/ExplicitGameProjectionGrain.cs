using System;
using BakerySim.Common.Orleans;
using BakerySim.Grains.Events;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace BakerySim.Grains.Projections;

/// <summary>
/// GameProjectionGrain using an explicit stream subscription.
/// This grain subscribes to the GameStartedEvent stream and handles the events after it has been activated.
/// </summary>.
public class ExplicitGameProjectionGrain : Grain, IExplicitGameProjectionGrain, IAsyncObserver<GameStartedEvent>
{
    private readonly ILogger<ExplicitGameProjectionGrain> logger;
    private StreamSubscriptionHandle<GameStartedEvent>? subscription;

    public ExplicitGameProjectionGrain(ILogger<ExplicitGameProjectionGrain> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Marker method to ensure the grain is activated.
    /// </summary>
    /// <returns></returns>
    public Task EnsureActivated() => Task.CompletedTask;

    /// <summary>
    /// OnActivateAsync is called when the grain is activated.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // Explicitly subscribe to the game events for the game that matches this grain's identity.
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        var streamId = StreamId.Create(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, this.GetPrimaryKey());
        var stream = streamProvider.GetStream<GameStartedEvent>(streamId);
        subscription = await stream.SubscribeAsync(this);
        await base.OnActivateAsync(cancellationToken);
    }

    /// <summary>
    /// OnErrorAsync is called when an error occurs while processing the stream.
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public Task OnErrorAsync(Exception ex)
    {
        // TODO: Implement error handling logic.
        logger.LogError(ex, "ExplicitGameProjectionGrain: Error occurred while processing the stream.");
        return Task.CompletedTask;
    }

    public Task OnNextAsync(GameStartedEvent item, StreamSequenceToken? token = null)
    {
        // TODO: Handle the event and update the projection state.
        logger.LogInformation("ExplicitGameProjectionGrain: Handling GameStartedEvent");
        return Task.CompletedTask;
    }
}

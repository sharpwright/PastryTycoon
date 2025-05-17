using System;
using BakerySim.Common.Orleans;
using BakerySim.Grains.Events;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace BakerySim.Grains.Projections;

public class GameProjectionGrain : Grain, IGameProjectionGrain, IAsyncObserver<GameStartedEvent>
{
    private readonly ILogger<GameProjectionGrain> logger;
    private StreamSubscriptionHandle<GameStartedEvent>? subscription;

    public GameProjectionGrain(ILogger<GameProjectionGrain> logger)
    {
        this.logger = logger;
    }

    public Task EnsureActivated() => Task.CompletedTask;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        var streamId = StreamId.Create(OrleansConstants.STREAM_GAME_NAMESPACE, this.GetPrimaryKey().ToString());
        var stream = streamProvider.GetStream<GameStartedEvent>(streamId);
        subscription = await stream.SubscribeAsync(this);
        await base.OnActivateAsync(cancellationToken);
    }

    public Task OnErrorAsync(Exception ex)
    {
        logger.LogError(ex, "Error in GameProjectionGrain");
        return Task.CompletedTask;
    }

    public Task OnNextAsync(GameStartedEvent item, StreamSequenceToken? token = null)
    {
        //TODO: Handle the event and update the projection state.
        logger.LogInformation("Handling GameStartedEvent");
        return Task.CompletedTask;
    }
}

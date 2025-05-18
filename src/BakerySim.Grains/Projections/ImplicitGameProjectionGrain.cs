using System;
using BakerySim.Common.Orleans;
using BakerySim.Grains.Events;
using BakerySim.Grains.Observers;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Streams.Core;

namespace BakerySim.Grains.Projections;

/// <summary>
/// Example of a grain using an implicit stream subscription.
/// A subscription is created on the entire namespace and all events are handled by the observer.
/// This grain will be automatically activated when events are published to the stream.
/// NOTE: calling this.GetPrimaryKey(); will return the producers grain's identity.
/// </summary>
[ImplicitStreamSubscription(OrleansConstants.STREAM_GAME_NAMESPACE)]
public class ImplicitGameProjectionGrain : Grain, IImplicitGameProjectionGrain, IStreamSubscriptionObserver
{
    private readonly ILogger<IImplicitGameProjectionGrain> logger;
    private readonly IGameStartedEventObserver gameStartedEventObserver;

    public ImplicitGameProjectionGrain(ILogger<IImplicitGameProjectionGrain> logger, IGameStartedEventObserver gameStartedEventObserver)
    {
        this.logger = logger;
        this.gameStartedEventObserver = gameStartedEventObserver;
    }

    /// <summary>
    /// Handles the subscription to the stream.
    /// </summary>
    /// <param name="handleFactory"></param>
    /// <returns></returns>
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        // Plug our IGameStartedEventObserver to the stream
        var handle = handleFactory.Create<GameStartedEvent>();

        // NOTE: without calling ResumeAsync the stream will 
        // not see any subscribers and will drop the data.
        await handle.ResumeAsync(gameStartedEventObserver);
    }
}

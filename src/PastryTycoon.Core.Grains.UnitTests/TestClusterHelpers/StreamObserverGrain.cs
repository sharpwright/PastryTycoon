using System;
using Orleans.Streams;

namespace PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;

public class StreamObserverGrain<TEvent> : Grain, IStreamObserverGrain<TEvent>, IAsyncObserver<TEvent>
{
    private readonly List<TEvent> _received = new();

    public async Task SubscribeAsync(string streamNamespace, string providerName)
    {
        var provider = this.GetStreamProvider(providerName);
        var stream = provider.GetStream<TEvent>(streamNamespace, this.GetPrimaryKey());
        await stream.SubscribeAsync(this);
    }

    public Task<List<TEvent>> GetReceivedEventsAsync() => Task.FromResult(_received);

    public Task OnNextAsync(TEvent item, StreamSequenceToken? token = null)
    {
        _received.Add(item);
        return Task.CompletedTask;
    }
    public Task OnCompletedAsync() => Task.CompletedTask;
    public Task OnErrorAsync(Exception ex) => Task.CompletedTask;
}

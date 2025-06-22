using System;
using Orleans.Streams;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

public class StreamObserverGrain<TEvent> : Grain, IStreamObserverGrain<TEvent>, IAsyncObserver<TEvent>
{
    private readonly List<TEvent> receivedEvents = [];
    private readonly List<string> logMessages = [];

    public Task<List<string>> GetLogMessagesAsync()
    {
        logMessages.Add("Getting log messages.");
        return Task.FromResult(logMessages);
    }

    public async Task SubscribeAsync(string streamNamespace, string providerName)
    {
        var provider = this.GetStreamProvider(providerName);
        var stream = provider.GetStream<TEvent>(streamNamespace, this.GetPrimaryKeyString());
        await stream.SubscribeAsync(this);
    }

    public Task<List<TEvent>> GetReceivedEventsAsync()
    {
        logMessages.Add($"Getting received events. Count: {receivedEvents.Count}");
        return Task.FromResult(receivedEvents);
    }

    public Task ClearReceivedEventsAsync()
    {
        receivedEvents.Clear();
        return Task.CompletedTask;
    }
    
    public Task OnNextAsync(TEvent item, StreamSequenceToken? token = null)
    {
        logMessages.Add($"Received event: {item}");
        receivedEvents.Add(item);
        return Task.CompletedTask;
    }
    public Task OnCompletedAsync() => Task.CompletedTask;
    public Task OnErrorAsync(Exception ex) => Task.CompletedTask;
}

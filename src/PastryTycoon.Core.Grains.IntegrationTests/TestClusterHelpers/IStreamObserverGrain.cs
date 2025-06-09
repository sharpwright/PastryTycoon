using System;
using Orleans.Providers.Streams.Generator;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

public interface IStreamObserverGrain<TEvent> : IGrainWithGuidKey
{
    Task<List<string>> GetLogMessagesAsync();
    Task SubscribeAsync(string streamNamespace, string providerName);
    Task<List<TEvent>> GetReceivedEventsAsync();
    Task ClearReceivedEventsAsync();
}

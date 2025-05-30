using System;

namespace PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;

public interface IStreamObserverGrain<TEvent> : IGrainWithGuidKey
{
    Task SubscribeAsync(string streamNamespace, string providerName);
    Task<List<TEvent>> GetReceivedEventsAsync();
    Task ClearReceivedEventsAsync();
}

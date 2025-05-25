using System;

namespace PastryTycoon.Grains.UnitTests.TestClusterHelpers;

public interface IStreamObserverGrain<TEvent> : IGrainWithGuidKey
{
    Task SubscribeAsync(string streamNamespace, string providerName);
    Task<List<TEvent>> GetReceivedEventsAsync();
}

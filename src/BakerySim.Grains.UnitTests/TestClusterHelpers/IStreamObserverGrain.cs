using System;
using BakerySim.Common.Events;

namespace BakerySim.Common.UnitTests.TestClusterHelpers;

public interface IStreamObserverGrain<TEvent> : IGrainWithGuidKey
{
    Task SubscribeAsync(string streamNamespace, string providerName);
    Task<List<TEvent>> GetReceivedEventsAsync();
}

using System;

namespace BakerySim.Common.UnitTests.TestClusterHelpers;

public static class StreamObserverGrainExtensions
{
    public static async Task<bool> WaitForReceivedEventsAsync<TEvent>(
        this IStreamObserverGrain<TEvent> observer,
        int timeoutMs = 2000,
        int pollIntervalMs = 50)
    {
        var start = DateTime.UtcNow;
        while ((DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
        {
            var events = await observer.GetReceivedEventsAsync();
            if (events != null && events.Count > 0)
                return true;
            await Task.Delay(pollIntervalMs);
        }
        return false;
    }
}

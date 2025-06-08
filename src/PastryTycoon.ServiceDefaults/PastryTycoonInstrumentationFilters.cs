using System;

namespace PastryTycoon.ServiceDefaults;

public static class PastryTycoonInstrumentationFilters
{
    public static bool IgnoreAzureQueuePolling(HttpRequestMessage httpRequestMessage)
    {
        var uri = httpRequestMessage.RequestUri;
        if (uri != null)
        {
            // Exclude Azure Queue Storage polling (including Azurite emulator)
            bool isQueueMessages = uri.AbsolutePath.EndsWith("/messages", StringComparison.OrdinalIgnoreCase);
            bool isLocalQueueHost =
                uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                uri.Host.Equals("host.docker.internal", StringComparison.OrdinalIgnoreCase);
            bool isAzureQueueHost = uri.Host.EndsWith(".queue.core.windows.net", StringComparison.OrdinalIgnoreCase);

            if (isQueueMessages && (isLocalQueueHost || isAzureQueueHost))
            {
                return false; // Do not trace these requests
            }
        }
        return true; // Trace all other requests
    }
}

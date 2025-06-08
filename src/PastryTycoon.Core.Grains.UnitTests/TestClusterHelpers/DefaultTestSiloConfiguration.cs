using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Grains.Game.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;

public sealed class DefaultTestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddMemoryStreams(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.STREAM_PUBSUB_STORE);
        siloBuilder.AddLogStorageBasedLogConsistencyProvider(OrleansConstants.EVENT_SOURCING_LOG_PROVIDER);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS);
        siloBuilder.ConfigureServices(static services =>
        {
            //services.AddSingleton(...);
        });
    }
}

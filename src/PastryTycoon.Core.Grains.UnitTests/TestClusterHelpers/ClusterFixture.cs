using System;
using PastryTycoon.Core.Abstractions.Constants;
using Orleans.TestingHost;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.Extensions.Configuration;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Grains.Game.Validators;
using PastryTycoon.Core.Abstractions.Game;

namespace PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;

public class ClusterFixture : IDisposable
{
    public TestCluster Cluster { get; } = new TestClusterBuilder()
        .AddSiloBuilderConfigurator<TestSiloConfigurations>()
        .Build();

    public ClusterFixture() => Cluster.Deploy();

    void IDisposable.Dispose() => Cluster.StopAllSilos();
}

file sealed class TestSiloConfigurations : ISiloConfigurator
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
            // Mock the InitializeGameStateCommandValidator to always succeed.
            // The validator has its own unit tests, so we can mock it here for simplicity.
            var mockValidation = new Mock<InitializeGameStateCommandValidator>();
            mockValidation
                .Setup(v => v.ValidateCommandAndThrowsAsync(It.IsAny<InitializeGameStateCommand>(), It.IsAny<GameState>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));
            
            services.AddSingleton(mockValidation.Object);
        });
    }
}

using System;
using PastryTycoon.Common.Constants;
using Orleans.TestingHost;
using FluentValidation;
using PastryTycoon.Common.Commands;
using PastryTycoon.Grains.Validation;
using Microsoft.Extensions.DependencyInjection;
using PastryTycoon.Grains.States;
using Moq;
using Microsoft.Extensions.Configuration;

namespace PastryTycoon.Grains.UnitTests.TestClusterHelpers;

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
        siloBuilder.ConfigureServices(static services =>
        {
            // Mock the InitializeGameStateCommandValidator to always succeed.
            var mockValidation = new Mock<InitializeGameStateCommandValidator>();
            mockValidation
                .Setup(v => v.ValidateCommandAsync(It.IsAny<InitializeGameStateCommand>(), It.IsAny<GameState>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            // Register the inline validator for the specific context type
            services.AddSingleton<InitializeGameStateCommandValidator>(mockValidation.Object);
        });
    }
}

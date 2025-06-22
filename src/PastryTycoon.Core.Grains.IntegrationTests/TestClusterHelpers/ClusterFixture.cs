using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Orleans.TestingHost;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

/// <summary>
/// Defines a fixture for the test cluster used in integration tests.
/// This fixture sets up the cluster with default configurations.
/// For specific grain configurations, use the <see cref="ClusterFactory"/>  
/// to create and deploy a cluster with custom configurators, instead of using this fixture.
/// </summary>
public class ClusterFixture : IAsyncLifetime
{
    private readonly IContainer azuriteContainer;
    public TestCluster Cluster { get; }

    public ClusterFixture()
    {
        azuriteContainer = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/azure-storage/azurite")
            .WithPortBinding(10000, 10000) // Blob Port
            .WithPortBinding(10001, 10001) // Queue Port
            .WithPortBinding(10002, 10002) // Table Port
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(10001))
            .Build();

        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<DefaultTestSiloConfigurations>();
        builder.AddClientBuilderConfigurator<DefaultClientBuilderConfigurations>();

        Cluster = builder.Build();
    }

    public async Task InitializeAsync()
    {
        await azuriteContainer.StartAsync();
        await Cluster.DeployAsync();
    }

    public async Task DisposeAsync()
    {
        await Cluster.StopAllSilosAsync();
        await azuriteContainer.StopAsync();
    }
}
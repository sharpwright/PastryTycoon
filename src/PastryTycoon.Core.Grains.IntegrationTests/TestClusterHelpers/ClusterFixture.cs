using System;
using Orleans.TestingHost;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

/// <summary>
/// Defines a fixture for the test cluster used in integration tests.
/// This fixture sets up the cluster with default configurations.
/// For specific grain configurations, use the <see cref="ClusterFactory"/>  
/// to create and deploy a cluster with custom configurators, instead of using this fixture.
/// </summary>
public class ClusterFixture : IDisposable
{
    public TestCluster Cluster { get; } = new TestClusterBuilder()
        .AddSiloBuilderConfigurator<DefaultTestSiloConfigurations>()
        .Build();

    public ClusterFixture() => Cluster.Deploy();

    void IDisposable.Dispose() => Cluster.StopAllSilos();
}
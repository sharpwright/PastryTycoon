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
using System.Numerics;
using PastryTycoon.Data.Recipes;

namespace PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;

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
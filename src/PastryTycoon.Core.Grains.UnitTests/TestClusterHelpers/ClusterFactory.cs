using System;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Constants;

namespace PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;
public static class ClusterFactory
{
    /// <summary>
    /// Creates a new <see cref="TestClusterBuilder"/> with the default silo configurations.
    /// </summary>
    /// <returns></returns>
    public static TestClusterBuilder CreateBuilder()
    {
        return new TestClusterBuilder()
            .AddSiloBuilderConfigurator<DefaultTestSiloConfigurations>();
    }
    
    /// <summary>
    /// Creates and deploys a new <see cref="TestCluster"/> with the specified configurations 
    /// (including the default configurations).
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static TestCluster CreateAndDeployCluster(Action<TestClusterBuilder> configure = null)
    {
        var builder = CreateBuilder();
        configure?.Invoke(builder);
        var cluster = builder.Build();
        cluster.Deploy();
        return cluster;
    }
}
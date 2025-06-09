using System;
using Orleans.TestingHost;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;
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
    /// (including the default configurations). Use this instead of <see cref="ClusterFixture" />
    /// to ensure that the cluster needs specific configuration not provided by <see cref="DefaultTestSiloConfigurations"/> .
    /// </summary>
    /// <example>
    ///     <code>
    ///         var cluster = ClusterFactory.CreateAndDeployCluster(builder =>
    ///         {
    ///            builder.AddSiloBuilderConfigurator&lt;MyCustomSiloConfigurator&gt;();
    ///            // Add any additional configurations here
    ///         });
    ///     9</code>
    /// </example>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static TestCluster CreateAndDeployCluster(Action<TestClusterBuilder>? configure = null)
    {
        var builder = CreateBuilder();
        configure?.Invoke(builder);
        var cluster = builder.Build();
        cluster.Deploy();
        return cluster;
    }
}
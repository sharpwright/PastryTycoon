using System;
using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Common;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

/// <summary>
/// Configures the test client to connect to the test silo.
/// It's crucial that the ClusterId and ServiceId match the silo's configuration.
/// </summary>
public sealed class DefaultClientBuilderConfigurations : IClientBuilderConfigurator
{
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        clientBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = OrleansConstants.CLUSTER_ID;
            options.ServiceId = OrleansConstants.SERVICE_ID;
        });
    }
}

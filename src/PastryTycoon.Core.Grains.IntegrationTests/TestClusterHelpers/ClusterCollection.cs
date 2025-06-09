using System;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

/// <summary>
/// Defines a collection fixture for the test cluster.
/// </summary>
[CollectionDefinition(Name)]
public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>
{
    public const string Name = nameof(ClusterCollection);
}
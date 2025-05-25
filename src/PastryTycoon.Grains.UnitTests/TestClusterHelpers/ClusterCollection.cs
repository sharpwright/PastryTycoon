using System;

namespace PastryTycoon.Grains.UnitTests.TestClusterHelpers;

[CollectionDefinition(Name)]
public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>
{
    public const string Name = nameof(ClusterCollection);
}

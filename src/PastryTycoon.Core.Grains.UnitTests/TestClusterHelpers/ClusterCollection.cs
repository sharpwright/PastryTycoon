using System;

namespace PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;

[CollectionDefinition(Name)]
public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>
{
    public const string Name = nameof(ClusterCollection);
}

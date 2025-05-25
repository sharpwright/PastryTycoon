using System;

namespace BakerySim.Common.UnitTests.TestClusterHelpers;

[CollectionDefinition(Name)]
public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>
{
    public const string Name = nameof(ClusterCollection);
}

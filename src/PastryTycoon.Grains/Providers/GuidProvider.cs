using System;

namespace PastryTycoon.Grains.Providers;

public class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.NewGuid();
}

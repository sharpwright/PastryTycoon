using System;

namespace PastryTycoon.Core.Grains.Common;

public class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.NewGuid();
}

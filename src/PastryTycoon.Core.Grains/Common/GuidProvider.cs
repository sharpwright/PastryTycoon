using System;

namespace PastryTycoon.Core.Grains.Common;

/// <summary>
/// Provides methods to generate new GUIDs.
/// </summary>
public class GuidProvider : IGuidProvider
{
    /// <summary>
    /// Generates a new globally unique identifier (GUID).
    /// </summary>
    /// <returns></returns>
    public Guid NewGuid() => Guid.NewGuid();
}

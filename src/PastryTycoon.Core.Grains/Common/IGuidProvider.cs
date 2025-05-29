using System;

namespace PastryTycoon.Core.Grains.Common;

/// <summary>
/// Provides a method to generate new GUIDs.
/// This interface is useful for testing purposes, allowing for easy mocking of GUID generation.
/// </summary>
public interface IGuidProvider
{
    /// <summary>
    /// Generates a new globally unique identifier (GUID).
    /// </summary>
    /// <returns></returns>
    Guid NewGuid();
}
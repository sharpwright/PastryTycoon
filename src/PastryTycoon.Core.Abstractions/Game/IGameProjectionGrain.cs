using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

/// <summary>
/// Interface for the Game Projection grain.
/// </summary>
[Alias("GameProjectionGrain")]
public interface IGameProjectionGrain : IGrainWithGuidKey
{

}

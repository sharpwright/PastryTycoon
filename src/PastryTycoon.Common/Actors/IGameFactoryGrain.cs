using System;
using Orleans;

namespace PastryTycoon.Common.Actors;

public interface IGameFactoryGrain : IGrainWithGuidKey
{
    /// <summary>
    /// Creates a new game for the specified player with the given game name.
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="gameName"></param>
    /// <returns></returns>
    Task<Guid> CreateNewGameAsync(Guid playerId, string gameName);
}

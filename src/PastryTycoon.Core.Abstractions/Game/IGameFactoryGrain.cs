using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

public interface IGameFactoryGrain : IGrainWithGuidKey
{
    /// <summary>
    /// Creates a new game for the specified player with the given game name.
    /// </summary>
    /// <param name="command">Command containing player ID, player name, and difficulty level.</param>
    /// <returns></returns>
    Task<Guid> CreateNewGameAsync(CreateNewGameCommand command);
}

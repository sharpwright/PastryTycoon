using System;
using Orleans;
using PastryTycoon.Core.Abstractions.Common;

namespace PastryTycoon.Core.Abstractions.Game;

[Alias("GameFactoryGrain")]
public interface IGameFactoryGrain : IGrainWithGuidKey
{
    /// <summary>
    /// Creates a new game for the specified player with the given game name.
    /// </summary>
    /// <param name="command">Command containing player ID, player name, and difficulty level.</param>
    /// <returns></returns>
    Task<CommandResult> CreateNewGameAsync(CreateNewGameCmd command);
}

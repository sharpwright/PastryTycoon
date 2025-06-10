using System;
using Orleans;
using PastryTycoon.Core.Abstractions.Common;

namespace PastryTycoon.Core.Abstractions.Game;

/// <summary>
/// Interface for the Game grain.
/// </summary>
[Alias("GameGrain")]
public interface IGameGrain : IGrainWithGuidKey
{
    /// <summary>
    /// Initializes the game state with the provided command.
    /// </summary>
    /// <param name="command">Command containing game initialization details.</param>
    /// <returns></returns>
    Task<CommandResult> InitializeGameStateAsync(InitGameStateCmd command);

    /// <summary>
    /// Adds a recipe to the game based on the provided command.
    /// </summary>
    /// <param name="command">Command containing the game ID and recipe ID to be added.</param>
    /// <returns></returns>
    Task<CommandResult> UpdateGameAsync(UpdateGameCmd command);

    /// <summary>
    /// Adds a recipe to the game based on the provided command.
    /// </summary>
    /// <returns></returns>
    Task<GameStatisticsDto> GetGameStatisticsAsync();
}

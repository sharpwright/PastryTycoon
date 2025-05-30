using System;

namespace PastryTycoon.Web.API.Game;

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public record StartGameRequest(string playerName, DifficultyLevel difficultyLevel);
public record UpdateGameRequest(Guid GameId, string GameName);
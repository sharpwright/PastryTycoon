using System;

namespace PastryTycoon.Web.API.Game;

public record StartGameRequest(string GameName);
public record UpdateGameRequest(Guid GameId, string GameName);
using System;
using Orleans;

namespace PastryTycoon.Common.Actors;

public interface IGameFactoryGrain : IGrainWithGuidKey
{
    Task<Guid> CreateNewGameAsync(Guid playerId, string gameName);
}

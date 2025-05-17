using System;
using BakerySim.Common.Orleans;
using BakerySim.Grains.Commands;
using BakerySim.Grains.States;

namespace BakerySim.Grains.Actors;

public class GameGrain : Grain, IGameGrain
{
    private readonly IPersistentState<GameState> state;

    public GameGrain(
        [PersistentState("game", OrleansConstants.AZURE_TABLE_GRAIN_STORAGE)]IPersistentState<GameState> state)
    {
        this.state = state;
    }

    public async Task StartGame(StartGameCommand command)
    {
        state.State.GameId = command.GameId;
        state.State.GameName = command.GameName;
        state.State.StartTime = command.StartTime;
        await state.WriteStateAsync();
    }
}

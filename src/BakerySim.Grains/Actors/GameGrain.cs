using System;
using BakerySim.Common.Orleans;
using BakerySim.Grains.Commands;
using BakerySim.Grains.Events;
using BakerySim.Grains.Projections;
using BakerySim.Grains.States;
using Orleans.Streams;

namespace BakerySim.Grains.Actors;

public class GameGrain : Grain, IGameGrain
{
    private readonly IPersistentState<GameState> state;
    private IAsyncStream<GameStartedEvent>? gameStartedStream;

    public GameGrain(
        [PersistentState("game", OrleansConstants.AZURE_TABLE_GRAIN_STORAGE)] IPersistentState<GameState> state)
    {
        this.state = state;
    }



    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // Create the stream to push events to.
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        var streamId = StreamId.Create(OrleansConstants.STREAM_GAME_NAMESPACE, this.GetPrimaryKey().ToString());
        gameStartedStream = streamProvider.GetStream<GameStartedEvent>(streamId);
        await base.OnActivateAsync(cancellationToken);

        // Ensure activation of the projection grain to handle events.
        var projectionGrain = GrainFactory.GetGrain<IGameProjectionGrain>(this.GetPrimaryKey());
        await projectionGrain.EnsureActivated();
    }


    public async Task StartGame(StartGameCommand command)
    {
        state.State.GameId = command.GameId;
        state.State.GameName = command.GameName;
        state.State.StartTime = command.StartTime;
        await state.WriteStateAsync();

        if (gameStartedStream != null)
        {
            var evt = new GameStartedEvent(command.GameId, command.GameName, command.StartTime);
            await gameStartedStream.OnNextAsync(evt);
        }
    }
}

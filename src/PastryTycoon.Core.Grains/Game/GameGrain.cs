using System;
using PastryTycoon.Core.Abstractions.Constants;
using Orleans.Providers;
using Orleans.Streams;
using Orleans.EventSourcing;
using FluentValidation;
using PastryTycoon.Core.Grains.Game.Validators;
using PastryTycoon.Core.Abstractions.Game;

namespace PastryTycoon.Core.Grains.Game;

[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS)]
public class GameGrain : JournaledGrain<GameState, GameEvent>, IGameGrain
{
    private IAsyncStream<GameEvent>? gameEventStream;

    public GameGrain()
    {
    }
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        gameEventStream = streamProvider.GetStream<GameEvent>(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, this.GetPrimaryKey());
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task InitializeGameStateAsync(InitializeGameStateCommand command)
    {
        // Validate the command.
        var initializeValidator = new InitializeGameStateCommandValidator();
        await initializeValidator.ValidateCommandAndThrowsAsync(command, State, this.GetPrimaryKey());

        var evt = new GameStateInitializedEvent(
            command.GameId,
            command.PlayerId,
            command.RecipeIds,
            command.StartTimeUtc);
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {
            await gameEventStream.OnNextAsync(evt);
        }        
    }

    public async Task UpdateGameAsync(UpdateGameCommand command)
    {
        var validator = new UpdateGameCommandValidator();
        await validator.ValidateCommandAndThrowsAsync(command, State, this.GetPrimaryKey());

        var evt = new GameUpdatedEvent(command.GameId, command.GameName, command.UpdateTimeUtc);
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {
            await gameEventStream.OnNextAsync(evt);
        }
    }
    
    public Task<GameStatisticsDto> GetGameStatisticsAsync()
    {
        if (!State.IsInitialized)
        {
            throw new InvalidOperationException("Game state is not initialized. Please initialize the game first.");
        }

        return Task.FromResult(new GameStatisticsDto
        {
            GameId = State.GameId,
            PlayerId = State.PlayerId,
            TotalRecipes = State.DiscoverableRecipeIds != null ? State.DiscoverableRecipeIds.Count : 0,
            StartTimeUtc = State.StartTimeUtc,
            LastUpdatedUtc = State.LastUpdatedAtTimeUtc
        });
    }
}

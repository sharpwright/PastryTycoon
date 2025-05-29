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
    private readonly InitializeGameStateCommandValidator initializeValidator;

    public GameGrain(InitializeGameStateCommandValidator initializeValidator)
    {
        this.initializeValidator = initializeValidator;
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
        await initializeValidator.ValidateCommandAsync(command, State, this.GetPrimaryKey());

        var evt = new GameStateInitializedEvent(command.GameId, command.PlayerId, command.RecipeIds, command.GameName, command.StartTimeUtc);
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {
            await gameEventStream.OnNextAsync(evt);
        }        
    }

    public async Task UpdateGameAsync(UpdateGameCommand command)
    {
        if (!command.GameId.Equals(this.GetPrimaryKey()))
        {
            throw new ArgumentException("Command GameId does not match grain primary key.");
        }

        var evt = new GameUpdatedEvent(command.GameId, command.GameName, command.UpdateTimeUtc);
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {
            await gameEventStream.OnNextAsync(evt);
        }
    }
    
    public Task<GameStatisticsDto> GetGameStatisticsAsync(Guid gameId)
    {
        if (!gameId.Equals(this.GetPrimaryKey()))
        {
            throw new ArgumentException("GameId does not match grain primary key.");
        }

        return Task.FromResult(new GameStatisticsDto
        {
            GameId = State.GameId,
            PlayerId = State.PlayerId,
            GameName = State.GameName,
            TotalRecipes = State.DiscoverableRecipeIds != null ? State.DiscoverableRecipeIds.Count : 0,
            StartTimeUtc = State.StartTimeUtc,
            LastUpdatedUtc = State.LastUpdatedAtTimeUtc
        });
    }
}

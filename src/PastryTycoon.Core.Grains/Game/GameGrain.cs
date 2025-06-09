using System;
using PastryTycoon.Core.Abstractions.Constants;
using Orleans.Providers;
using Orleans.Streams;
using Orleans.EventSourcing;
using PastryTycoon.Core.Grains.Game.Validators;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Grain that represents a game instance in the Pastry Tycoon application.
/// </summary>
[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS)]
public class GameGrain : JournaledGrain<GameState, GameEvent>, IGameGrain
{
    private IAsyncStream<GameEvent>? gameEventStream;
    private readonly IGrainValidator<InitializeGameStateCommand, GameState, Guid> initializeValidator;
    private readonly IGrainValidator<UpdateGameCommand, GameState, Guid> updateValidator;

    public GameGrain(
        IGrainValidator<InitializeGameStateCommand, GameState, Guid> initializeValidator,
        IGrainValidator<UpdateGameCommand, GameState, Guid> updateValidator)
    {
        this.initializeValidator = initializeValidator;
        this.updateValidator = updateValidator;
    }
    
    /// <summary>
    /// Called when the grain is activated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for activation.</param>
    /// <returns></returns>
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        gameEventStream = streamProvider.GetStream<GameEvent>(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, this.GetPrimaryKey());
        await base.OnActivateAsync(cancellationToken);
    }

    /// <summary>
    /// Initializes the game state based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the game initialization details.</param>
    /// <returns></returns>
    public async Task InitializeGameStateAsync(InitializeGameStateCommand command)
    {
        // Validate the command.
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
    
    /// <summary>
    /// Updates the game state based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the game update details.</param>
    /// <returns></returns>
    public async Task UpdateGameAsync(UpdateGameCommand command)
    {
        await updateValidator.ValidateCommandAndThrowsAsync(command, State, this.GetPrimaryKey());

        var evt = new GameUpdatedEvent(command.GameId, command.UpdateTimeUtc);
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {
            await gameEventStream.OnNextAsync(evt);
        }
    }
    
    /// <summary>
    /// Retrieves the game statistics for the current game instance.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the game state is not initialized.</exception>
    public Task<GameStatisticsDto> GetGameStatisticsAsync()
    {
        if (!State.IsInitialized)
        {
            throw new InvalidOperationException("Game state is not initialized. Please initialize the game first.");
        }

        return Task.FromResult(new GameStatisticsDto(
            State.GameId,
            State.PlayerId,
            State.DiscoverableRecipeIds != null ? State.DiscoverableRecipeIds.Count : 0,
            State.StartTimeUtc,
            State.LastUpdatedAtTimeUtc));
    }
}

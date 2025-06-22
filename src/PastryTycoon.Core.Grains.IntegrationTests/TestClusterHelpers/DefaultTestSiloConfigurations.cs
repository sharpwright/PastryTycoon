using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.CommandHandlers;
using PastryTycoon.Core.Grains.Player.Validators;
using PastryTycoon.Data.Ingredients;
using PastryTycoon.Data.Recipes;
using FluentValidation;
using PastryTycoon.Core.Grains.Game.Validators;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Grains.Game.CommandHandlers;
using PastryTycoon.Core.Abstractions.Common;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

public sealed class DefaultTestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddMemoryStreams(OrleansConstants.STREAM_PROVIDER_NAME);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.STREAM_PUBSUB_STORE);
        siloBuilder.AddLogStorageBasedLogConsistencyProvider(OrleansConstants.EVENT_SOURCING_LOG_PROVIDER);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS);
        siloBuilder.ConfigureServices(static services =>
        {
            // Add default services for the test cluster.
            services.AddSingleton(SetupRecipeRepositoryMock().Object);
            services.AddSingleton<IIngredientRepository, IngredientRepository>();
            services.AddSingleton<IGuidProvider, GuidProvider>();

            // Add game grain validators and command handlers.
            services.AddSingleton<IValidator<CreateNewGameCmd>, CreateNewGameCmdVal>();
            services.AddSingleton<IValidator<InitGameStateCmd>, InitGameStateCmdVal>();
            services.AddSingleton<IValidator<UpdateGameCmd>, UpdateGameCmdVal>();
            services.AddSingleton<ICommandHandler<InitGameStateCmd, GameState, Guid, GameEvent>, InitGameStateCmdHdlr>();
            services.AddSingleton<ICommandHandler<UpdateGameCmd, GameState, Guid, GameEvent>, UpdateGameCmdHdlr>();
                        
            // Add player grain command handlers and validators.
            services.AddSingleton<IValidator<InitPlayerCmd>, InitPlayerCmdVal>();
            services.AddSingleton<IValidator<TryDiscoverRecipeCmd>, TryDiscoverRecipeCmdVal>();
            services.AddSingleton<IValidator<UnlockAchievementCmd>, UnlockAchievementCmdVal>();
            services.AddSingleton<ICommandHandler<InitPlayerCmd, PlayerState, Guid, PlayerEvent>, InitPlayerCmdHdlr>();
            services.AddSingleton<ICommandHandler<TryDiscoverRecipeCmd, PlayerState, Guid, PlayerEvent>, TryDiscoverRecipeCmdHdlr>();
            services.AddSingleton<ICommandHandler<UnlockAchievementCmd, PlayerState, Guid, PlayerEvent>, UnlockAchievementCmdHdlr>();
        });
    }

    private static Mock<IRecipeRepository> SetupRecipeRepositoryMock()
    {
        var mock = new Mock<IRecipeRepository>();
        var testRecipe = new Recipe(
            "test-recipe-id",
            "Test Recipe",
            [
                new RecipeIngredient("test-ingredient-1", null, 1),
                new RecipeIngredient("test-ingredient-2", null, 2)
            ]);

        // Setup default behavior for GetRecipeByIngredientIdsAsync
        mock.Setup(r => r.GetRecipeByIngredientIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(testRecipe);

        // Add any other method setups for IRecipeRepository
        mock.Setup(r => r.GetRecipeByIdAsync(testRecipe.Id))
            .ReturnsAsync(testRecipe);

        return mock;
    }
}

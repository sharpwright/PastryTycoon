using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Grains.Game.Validators;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.CommandHandlers;
using PastryTycoon.Data.Ingredients;
using PastryTycoon.Data.Recipes;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

public sealed class DefaultTestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddMemoryStreams(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.STREAM_PUBSUB_STORE);
        siloBuilder.AddLogStorageBasedLogConsistencyProvider(OrleansConstants.EVENT_SOURCING_LOG_PROVIDER);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS);
        siloBuilder.AddMemoryGrainStorage(OrleansConstants.EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS);
        siloBuilder.ConfigureServices(static services =>
        {
            // Add xUnit test output helper by default.
            services.AddSingleton<ITestOutputHelper, TestOutputHelper>();

            // Add default services for the test cluster.
            services.AddSingleton(SetupRecipeRepositoryMock().Object);
            services.AddSingleton<IIngredientRepository, IngredientRepository>();
            services.AddSingleton<IGuidProvider, GuidProvider>();
            services.AddSingleton<InitializeGameStateCommandValidator>();
            services.AddTransient<ICommandHandler<TryDiscoverRecipeCommand, PlayerEvent, PlayerState>, TryDiscoverRecipeCommandHandler>();
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

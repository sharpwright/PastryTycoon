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
using Azure.Storage.Queues;
using Azure.Data.Tables;
using Orleans.Configuration;

namespace PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;

public sealed class DefaultTestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        var storageConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        // CONFIGURE CLUSTERING: use Azure Storage for clustering.
        siloBuilder.UseAzureStorageClustering(options =>
        {
            options.TableServiceClient = new TableServiceClient(storageConnectionString);
        });

        // CONFIGURE CLUSTER
        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = OrleansConstants.CLUSTER_ID;
            options.ServiceId = OrleansConstants.SERVICE_ID;
        });

        // CONFIGURE GRAIN STORAGE: add grain state persistence using Azure Table Storage.
        // UNCOMMENT: when you want a regular Grain to use Table Storage for state persistence.
        siloBuilder.AddAzureTableGrainStorage(OrleansConstants.GRAIN_STATE_ACHIEVEMENTS, options =>
        {
            options.TableServiceClient = new TableServiceClient(storageConnectionString);
            options.UseStringFormat = false;
            options.TableName = OrleansConstants.GRAIN_STATE_ACHIEVEMENTS;
        });

        // CONFIGURE STREAMING API: add streaming using Azure Queue Storage.
        siloBuilder.AddAzureQueueStreams(OrleansConstants.STREAM_PROVIDER_NAME, optionsBuilder =>
        {
            optionsBuilder.Configure(options =>
            {
                options.QueueServiceClient = new QueueServiceClient(storageConnectionString);
            });
        });

        // CONFIGURE STREAMING API: add PubSubStore using Azure Table Storage.
        // Must be named "PubSubStore" so that Orleans can find and use it by convention or configuration.
        siloBuilder.AddAzureTableGrainStorage(OrleansConstants.STREAM_PUBSUB_STORE, options =>
        {
            options.TableServiceClient = new TableServiceClient(storageConnectionString);
            options.TableName = OrleansConstants.STREAM_PUBSUB_STORE;
            options.UseStringFormat = false;
        });

        // CONFIGURE EVENT SOURCING: add consistency provider to store all events in a log.
        siloBuilder.AddLogStorageBasedLogConsistencyProvider(OrleansConstants.EVENT_SOURCING_LOG_PROVIDER);

        // CONFIGURE EVENT SOURCING: add GameEvent log storage using Azure Table Storage. 
        siloBuilder.AddAzureTableGrainStorage(OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS, options =>
        {
            options.TableServiceClient = new TableServiceClient(storageConnectionString);
            options.TableName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS;
            options.UseStringFormat = true;
        });

        // CONFIGURE EVENT SOURCING: add GameEvent log storage using Azure Table Storage. 
        siloBuilder.AddAzureTableGrainStorage(OrleansConstants.EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS, options =>
        {
            options.TableServiceClient = new TableServiceClient(storageConnectionString);
            options.TableName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS;
            options.UseStringFormat = true;
        });

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

using Azure.Data.Tables;
using Azure.Storage.Queues;
using BakerySim.Common.Orleans;
using BakerySim.Grains.Observers;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;


await Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // IAsyncObservers are not managed by Orleans and need to be added to the DI container.
        services.AddSingleton<IGameStartedEventObserver, GameStartedEventObserver>();
    })
    .UseOrleans(static siloBuilder =>
    {
        // CONFIGURE CLUSTERING: use Azure Storage for clustering.
        siloBuilder.UseAzureStorageClustering(options =>
        {
            options.TableServiceClient = new TableServiceClient(OrleansConstants.STORAGE_CONNECTION_STRING);
        });

        // CONFIGURE CLUSTER
        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = OrleansConstants.CLUSTER_ID;
            options.ServiceId = OrleansConstants.SERVICE_ID;
        });

        // CONFIGURE GRAIN STORAGE: add grain state persistence using Azure Table Storage.
        siloBuilder.AddAzureTableGrainStorage(OrleansConstants.AZURE_TABLE_GRAIN_STORAGE, options =>
        {
            options.TableServiceClient = new TableServiceClient(OrleansConstants.STORAGE_CONNECTION_STRING);
            options.UseStringFormat = false;
        });

        // CONFIGURE STREAMING API: add streaming using Azure Queue Storage.
        siloBuilder.AddAzureQueueStreams(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER, optionsBuilder =>
        {
            optionsBuilder.Configure(options =>
            {
                options.QueueServiceClient = new QueueServiceClient(OrleansConstants.STORAGE_CONNECTION_STRING);
            });
        });

        // CONFIGURE STREAMING API: add PubSub store using Azure Table Storage.
        siloBuilder.AddAzureTableGrainStorage(OrleansConstants.AZURE_TABLE_PUBSUB_STORAGE, options =>
        {
            options.TableServiceClient = new TableServiceClient(OrleansConstants.STORAGE_CONNECTION_STRING);
            options.UseStringFormat = false;
        });

        // CONFIGURE LOGGING
        siloBuilder.ConfigureLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });          

        // Configure how often Grains need to be cleaned up from the cluster.
        // siloBuilder.Configure<GrainCollectionOptions>(options =>
        // {
        //     options.CollectionQuantum = TimeSpan.FromSeconds(30);
        //     options.CollectionAge = TimeSpan.FromMinutes(5); 
        // });
    })
    .RunConsoleAsync();
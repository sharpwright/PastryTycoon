using Azure.Data.Tables;
using BakerySim.Common.Orleans;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;


await Host.CreateDefaultBuilder(args)
    .UseOrleans(static siloBuilder =>
    {
        // Configure Orleans server to use Azure Storage for clustering.
        siloBuilder.UseAzureStorageClustering(configureOptions: options =>
        {
            options.TableServiceClient = new TableServiceClient(OrleansConstants.STORAGE_CONNECTION_STRING);
        });

        // Configure Orleans Server Cluster Options.
        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = OrleansConstants.CLUSTER_ID;
            options.ServiceId = OrleansConstants.SERVICE_ID;
        });

        // Configure Grain State persistence using Azure Table Storage.
        siloBuilder.AddAzureTableGrainStorage(OrleansConstants.AZURE_TABLE_GRAIN_STORAGE, configureOptions: options =>
        {
            options.TableServiceClient = new TableServiceClient(OrleansConstants.STORAGE_CONNECTION_STRING);
            options.UseStringFormat = false;
        });

        // Configure the logging level for the Orleans server.
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
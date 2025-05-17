using Azure.Data.Tables;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;


await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        // Configure Orleans server to use Azure Storage for clustering.
        siloBuilder.UseAzureStorageClustering(configureOptions: options =>
        {
            options.TableServiceClient = new TableServiceClient("UseDevelopmentStorage=true");
            options.TableName = "Grains";
        });

        // Configure Orleans Server Cluster Options.
        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "BakerySimCluster";
            options.ServiceId = "BakerySim";
        });

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
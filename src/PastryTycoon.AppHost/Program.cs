using System.Numerics;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Client.Extensions.Msal;
using Aspire.Hosting.Azure;
using YamlDotNet.Serialization; // Add this for Azure Aspire extensions

var builder = DistributedApplication.CreateBuilder(args);


var storage = builder.AddAzureStorage("storage")
       .RunAsEmulator(emulatorResource =>
       {
              // The emulatorResource is an IDockerContainerResourceBuilder
              // Add the --silent argument to Azurite's startup command
              emulatorResource.WithArgs("--silent");
       });

var streamProvider = storage.AddQueues("StreamProvider");
var clusteringTable = storage.AddTables("OrleansClustering");
var achievements = storage.AddTables("Achievements");
var pubSubStore = storage.AddTables("PubSubStore");
var gameEventLog = storage.AddTables("GameEventLog");
var playerEventLog = storage.AddTables("PlayerEventLog");

// var tableClient = storage.AddTables("tableClient");
// var queueClient = storage.AddQueues("queueClient");

// Add the Orleans resource to the Aspire DistributedApplication builder.
// Don't use WithClusteringTable() or WithGrainStorage() here, they are
// configured explicitly in the silo project.
var orleans = builder.AddOrleans("default")
       .WithClustering(clusteringTable)
       .WithClusterId("PastryTycoonCluster")
       .WithServiceId("PastryTycoonService")
       .WithStreaming("StreamProvider", streamProvider)
       .WithGrainStorage("Achievements", achievements)
       .WithGrainStorage("PubSubStore", pubSubStore)
       .WithGrainStorage("GameEventLog", gameEventLog)
       .WithGrainStorage("PlayerEventLog", playerEventLog);

// Add server project and reference the 'orleans' resource from it.
// It can join the Orleans cluster as a silo.
// This implicitly adds references to the required resources.
// In this case, that is the 'clusteringTable' resource declared earlier.
builder.AddProject<Projects.PastryTycoon_Silo>("silo")
       .WithReference(orleans)
       .WithReplicas(1);

// Reference the Orleans resource as a client from the 'frontend'
// project so that it can connect to the Orleans cluster.
builder.AddProject<Projects.PastryTycoon_Web_API>("web-api")
       .WithReference(orleans.AsClient())
       .WithExternalHttpEndpoints()
       .WithReplicas(1);

builder.Build().Run();

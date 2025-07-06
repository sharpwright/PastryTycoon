var builder = DistributedApplication.CreateBuilder(args);

// Azurite container
var azurite = builder.AddContainer("azurite", "mcr.microsoft.com/azure-storage/azurite")
    .WithVolume("azurite-data", "/data")
    .WithEndpoint(port: 10000, targetPort: 10000, name: "blob")
    .WithEndpoint(port: 10001, targetPort: 10001, name: "queue")
    .WithEndpoint(port: 10002, targetPort: 10002, name: "table");

// Cosmos DB Emulator container
var cosmosdb = builder.AddContainer("cosmosdb-emulator", "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator")
    .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "2")
    .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "1")
    .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
    .WithVolume("cosmos-data", "/tmp/cosmos/appdata")
    .WithEndpoint(port: 8081, targetPort: 8081, name: "https")
    .WithEndpoint(port: 10251, targetPort: 10251, name: "internal-gateway-1")
    .WithEndpoint(port: 10252, targetPort: 10252, name: "internal-gateway-2")
    .WithEndpoint(port: 10253, targetPort: 10253, name: "internal-gateway-3")
    .WithEndpoint(port: 10254, targetPort: 10254, name: "internal-gateway-4")
    .WithEndpoint(port: 10255, targetPort: 10255, name: "internal-gateway-5");

// NOTE: not using the Aspire.Hosting.Orleans package here.
// It can be used to configure Orleans clusters, but it seems unable to configure
// Azure Storage to use an external Azurite Docker container, so instead we use
// an Azurite container directly and configure the Silo project manually.

// Use an external/local Azurite Docker container instead of Aspire-managed emulator
var storage = builder.AddConnectionString("Storage");

// Configure Silo project to use Azure Storage.
var silo = builder.AddProject<Projects.PastryTycoon_Silo>("silo")
       .WaitFor(azurite)
       .WithReference(storage)
       .WithReplicas(1);

// Configure Web API project to use Azure Storage and Silo.
builder.AddProject<Projects.PastryTycoon_Web_API>("web-api")
       .WaitFor(silo)
       .WithReference(silo)
       .WithReference(storage)
       .WithExternalHttpEndpoints()
       .WithReplicas(1);

// Configure Web UI project to use Silo.
builder.AddProject<Projects.PastryTycoon_Web_UI>("web-ui")
       .WaitFor(silo)
       .WithReference(silo)
       .WithReference(storage)       
       .WithExternalHttpEndpoints();       

builder.Build().Run();

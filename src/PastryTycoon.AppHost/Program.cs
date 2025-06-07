using System.Numerics;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Client.Extensions.Msal;
using Aspire.Hosting.Azure;
using YamlDotNet.Serialization;
using Microsoft.Extensions.Configuration; // Add this for Azure Aspire extensions

var builder = DistributedApplication.CreateBuilder(args);

// Use an external/local Azurite Docker container instead of Aspire-managed emulator
var storage = builder.AddConnectionString("Storage");

// Configure Silo project to use Azure Storage.
var silo = builder.AddProject<Projects.PastryTycoon_Silo>("silo")
       .WithReference(storage)
       .WithReplicas(1);

// Configure Web API project to use Azure Storage and Silo.
builder.AddProject<Projects.PastryTycoon_Web_API>("web-api")
       .WithReference(storage)
       .WithReference(silo)
       .WithExternalHttpEndpoints()
       .WithReplicas(1);

builder.Build().Run();

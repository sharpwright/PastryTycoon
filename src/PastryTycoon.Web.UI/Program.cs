using Azure.Data.Tables;
using Orleans.Configuration;
using PastryTycoon.Core.Abstractions.Common;
using PastryTycoon.Web.UI.Components;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire UI services and configure OpenTelemetry.
builder.AddServiceDefaults();

// Add Orleans client services.
builder.Host.UseOrleansClient((context, client) =>
{           
    var storageConnectionString = builder.Configuration.GetConnectionString("Storage");

    // Configure Orleans client to be able to find Orleans clusters.
    client.UseAzureStorageClustering(configureOptions: options =>
    {
        options.TableServiceClient = new TableServiceClient(storageConnectionString);
    });

    // Configure Cluster Options, needs to match the silo options.
    client.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = OrleansConstants.CLUSTER_ID;
        options.ServiceId = OrleansConstants.SERVICE_ID;
    });

    // Configure activity propagation for OpenTelemetry.
    client.AddActivityPropagation();
    
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

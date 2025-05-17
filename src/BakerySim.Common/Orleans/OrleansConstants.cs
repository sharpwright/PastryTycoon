using System;

namespace BakerySim.Common.Orleans;

public static class OrleansConstants
{
    public const string CLUSTER_ID = "BakerySimCluster";
    public const string SERVICE_ID = "BakerySim";
    public const string AZURE_TABLE_GRAIN_STORAGE = "GrainState";
    public const string STORAGE_CONNECTION_STRING = "UseDevelopmentStorage=true";  
}
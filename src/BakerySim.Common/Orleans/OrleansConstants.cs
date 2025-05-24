using System;

namespace BakerySim.Common.Orleans;

public static class OrleansConstants
{
    public const string CLUSTER_ID = "BakerySimCluster";
    public const string SERVICE_ID = "BakerySim";
    public const string AZURE_STORAGE_CONNECTION_STRING = "UseDevelopmentStorage=true";
    public const string AZURE_TABLE_GRAIN_STORAGE = "GrainState";
    public const string AZURE_QUEUE_STREAM_PROVIDER = "StreamProvider";
    public const string STREAM_PUBSUB_STORE = "PubSubStore";
    public const string STREAM_NAMESPACE_GAME_EVENTS = "GameEventStream";
    public const string EVENT_SOURCING_LOG_PROVIDER = "EventLog";
    public const string EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS = "GameEventLog";
}
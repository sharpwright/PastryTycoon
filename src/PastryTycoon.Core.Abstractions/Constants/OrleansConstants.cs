using System;

namespace PastryTycoon.Core.Abstractions.Constants;

/// <summary>
/// Defines constants used throughout the Orleans-based Pastry Tycoon application.
/// </summary>
public static class OrleansConstants
{
    public const string CLUSTER_ID = "BakerySimCluster";
    public const string SERVICE_ID = "BakerySim";
    public const string AZURE_STORAGE_CONNECTION_STRING = "UseDevelopmentStorage=true";
    public const string AZURE_QUEUE_STREAM_PROVIDER = "StreamProvider";
    public const string GRAIN_STATE_ACHIEVEMENTS = "Achievements";
    public const string STREAM_PUBSUB_STORE = "PubSubStore";
    public const string STREAM_NAMESPACE_GAME_EVENTS = "GameEventStream";
    public const string STREAM_NAMESPACE_PLAYER_EVENTS = "PlayerEventStream";
    public const string EVENT_SOURCING_LOG_PROVIDER = "EventLog";
    public const string EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS = "GameEventLog";
    public const string EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS = "PlayerEventLog";
}
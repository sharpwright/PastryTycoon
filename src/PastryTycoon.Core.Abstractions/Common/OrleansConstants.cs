using System;

namespace PastryTycoon.Core.Abstractions.Common;

/// <summary>
/// Defines constants used throughout the Orleans-based Pastry Tycoon application.
/// </summary>
public static class OrleansConstants
{
    public const string CLUSTER_ID = "PastryTycoonCluster";
    public const string SERVICE_ID = "PastryTycoonService";
    public const string GRAIN_STATE_ACHIEVEMENTS = "Achievements";
    public const string EVENT_SOURCING_LOG_PROVIDER = "EventLog";
    public const string EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS = "GameEventLog";
    public const string EVENT_SOURCING_LOG_STORAGE_PLAYER_EVENTS = "PlayerEventLog";
    public const string STREAM_PROVIDER_NAME = "StreamProvider";
    public const string STREAM_PUBSUB_STORE = "PubSubStore";
    public const string STREAM_NAMESPACE_GAME_EVENTS = "Game.Events";
    public const string STREAM_NAMESPACE_PLAYER_EVENTS = "Player.Events";
}
using System;

namespace PastryTycoon.Core.Grains.Saga;

/// <summary>
/// Base class for operation events.
/// </summary>
/// <param name="OperationId"></param>
[GenerateSerializer]
public record OperationEvent(Guid OperationId);

/// <summary>
/// Event that is triggered when an operation is added.
/// </summary>
/// <param name="OperationId"></param>
/// <param name="Name"></param>
[GenerateSerializer]
public record OperationAddedEvent(Guid OperationId, string Name) : OperationEvent(OperationId);
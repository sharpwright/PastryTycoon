using System;

namespace PastryTycoon.Core.Grains.Saga;

using System.Collections.Generic;
using Orleans;
using Orleans.Concurrency;

[GenerateSerializer]
public class OperationState
{
    [Id(0)] public Guid OperationId { get; set; }

    [Id(1)] public string Name { get; set; } = string.Empty;

    [Id(5)] public DateTime CreatedAt { get; set; }

    [Id(6)] public DateTime UpdatedAt { get; set; }


    public void Apply(OperationAddedEvent evt)
    {
        OperationId = evt.OperationId;
        Name = evt.Name;
    }
}

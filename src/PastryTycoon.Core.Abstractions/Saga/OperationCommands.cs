using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Saga;

[GenerateSerializer]
public record SaveOperationCommand(Guid OperationId, string Name, IList<string> Activities);

[GenerateSerializer]
public record AddOperationCommand(Guid OperationId, string Name);

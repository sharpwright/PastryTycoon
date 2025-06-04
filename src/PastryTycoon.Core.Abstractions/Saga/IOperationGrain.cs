using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Saga;

public interface IOperationGrain : IGrainWithGuidKey
{
    Task AddOperation(AddOperationCommand command);
}

using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Common;

[GenerateSerializer]
public record BaseCommand(Guid CommandId);

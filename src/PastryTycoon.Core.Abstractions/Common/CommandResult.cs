using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Common;

[GenerateSerializer]
public class CommandResult
{
    [Id(0)] public bool IsSuccess { get; set; }
    [Id(2)] public List<string> Errors { get; set; } = new();
    public static CommandResult Success() => new() { IsSuccess = true };
    public static CommandResult Failure(params string[] errors) => new() { IsSuccess = false, Errors = errors.ToList() };
}

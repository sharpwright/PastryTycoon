using System;

namespace PastryTycoon.Core.Grains.Common;

public class CommandHandlerResult<TEventBase>
{
    public bool IsSuccess { get; set; }
    public TEventBase Event { get; set; } = default!;
    public IList<string> Errors { get; set; } = [];
    public static CommandHandlerResult<TEventBase> Success() => new() { IsSuccess = true };
    public static CommandHandlerResult<TEventBase> Success(TEventBase evt) => new() { IsSuccess = true, Event = evt };
    public static CommandHandlerResult<TEventBase> Failure(params string[] errors) => new() { IsSuccess = false, Errors = [.. errors] };
}

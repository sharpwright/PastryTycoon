using System;
using System.Diagnostics;
using PastryTycoon.Grains.Events;

namespace PastryTycoon.Grains.CommandHandlers;

public class CommandResult
{
    public bool IsSuccess { get; }
    private IEvent? Event { get; }
    public string? ErrorMessage { get; }

    private CommandResult(bool isSuccess, IEvent? evt = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        Event = evt;
        ErrorMessage = errorMessage;
    }

    public static CommandResult Success(IEvent evt) => new(true, evt);
    public static CommandResult Failure(string message) => new(false, errorMessage: message);
    public TEvent GetEvent<TEvent>() where TEvent : IEvent =>
        Event is TEvent evt
            ? evt
            : throw new InvalidOperationException($"Expected event of type {typeof(TEvent).Name}, but got {Event?.GetType().Name ?? "null"}.");
}

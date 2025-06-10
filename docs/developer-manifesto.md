# Pastry Tycoon Developer Manifesto

**Welcome to Pastry Tycoon!**  
By following these practices, you’ll help us build a robust, maintainable, and enjoyable codebase for everyone.

## 1. Project Philosophy

- **Clarity, Consistency, and Maintainability** are our guiding principles.
- Favor **explicitness over cleverness** and **robustness over shortcuts**.
- All code should be **testable, observable, and easy to reason about**.

## 2. Solution Structure

- **Separation of Concerns:**  
  - Grains, state, events, command handlers, and validators are in distinct folders and namespaces.
  - Shared contracts (commands, DTOs) live in `PastryTycoon.Core.Abstractions`.
  - Internal contracts (events, state) live in `PastryTycoon.Core.Grains`.
- **Tests:**  
  - Unit tests for handlers/validators.
  - Integration tests for grains using a real Orleans test silo.

## 3. Orleans Best Practices

- **Grain Design:**
  - Grains should be thin orchestrators, delegating business logic to handlers.
  - All grain methods that modify state should have a `Command` parameter and use `CommandResult` as return-type.
  - Never put complex business logic directly in grains.
  - Use `JournaledGrain` for event-sourced entities; use `[Alias]` on all persisted event and state types.
- **Grain Interfaces:**  
  - Always use `[Alias]` on grain interfaces and state classes for versioning and serialization stability.
- **Primary Keys:**  
  - Pass the strongly-typed primary key as a method parameter to handlers. Use `string` if supporting multiple key types.
- **Event Publishing:**  
  - Always call `ConfirmEvents()` before publishing events to streams.
  - Only publish events to streams that are meant for external subscribers.

## 4. Command Handlers & Validators

- **Handlers:**
  - Encapsulate all business logic and event creation.
  - Assume input is valid; validation is performed before calling the handler.
  - Return a result object (`CommandHandlerResult<TEvent>`) indicating success or failure.
  - Abbreviate as 'Hdlr' if brevity is needed, or use the full word for clarity.
- **Validators:**
  - Use FluentValidation for all command validation.
  - Place validators in the same or adjacent namespace as their handler.
  - Abbreviate as `Val` if brevity is needed, or use the full word for clarity.

## 5. Dependency Injection

- **Constructor Injection:**  
  - Inject only what is needed for the grain’s core responsibilities.
  - Use factories to avoid constructor bloat when many handlers/validators are needed.
- **No Service Locator:**  
  - Do not inject `IServiceProvider` for ad-hoc resolution except in rare, well-documented cases.

## 6. Error Handling

- **Validation and Business Errors:**  
  - Use result objects (`CommandResult`, `CommandHandlerResult<T>`) to communicate errors, not exceptions.
- **Unexpected Exceptions:**  
  - Use a single `try-catch` at the orchestration level to log and return a generic failure.
- **Logging:**  
  - Log all failures with enough context for diagnostics.

## 7. Testing

- **Handlers and Validators:**  
  - Unit test all business logic and validation in isolation.
- **Grains:**  
  - Use integration tests with a real Orleans test silo for any `JournaledGrain`.
  - Use unit tests with OrleansTestkit for any `Grain` to verify behaviour.

## 8. Serialization, Versioning & Compatibility

- **[Alias] Usage:**  
  - Annotate all grain interfaces, state classes, and persisted event types with `[Alias]`.
  - Do not use `[Alias]` on grain implementation classes.
- **[GenerateSerializer]:**  
  - Use `[GenerateSerializer]` on all types persisted or sent over the wire.

## 9. Naming Conventions

- **Handlers:**  
  - Use `Hdlr` as the abbreviation (e.g., `UnlockAchievementCmdHdlr`).
- **Validators:**  
  - Use `Val` or `Validator` (e.g., `UnlockAchievementCmdVal`).
- **Events:**  
  - Use clear, descriptive names and annotate with `[Alias]` if persisted (e.g. journaled grains events).
- **[Version] Usage:**  
  - Use `[Version(n)]` on properties of grain state, event, or DTO classes that may change over time and are persisted or shared, to enable safe versioning and schema evolution in Orleans.
  - When adding a new property to a persisted type, annotate it with `[Version(n)]` where `n` is the version number when it was introduced.
  - Example:
    ```csharp
    [GenerateSerializer]
    public class PlayerState
    {
        [Id(0)] public Guid PlayerId { get; set; }
        [Id(1)] public string PlayerName { get; set; } = string.Empty;
        [Id(2), Version(1)] public Guid GameId { get; set; } // Added in version 1
        [Id(3), Version(2)] public string? NewProperty { get; set; } // Added in version 2
    }
    ```
  - Use `[Version]` especially for grain state and event types that are persisted or may evolve.


## 10. General Coding Do’s and Don’ts

### Do:
- Write clear, self-documenting code.
- Keep methods short and focused.
- Use async/await for all Orleans grain methods.
- Document public APIs and grain methods.
- Use immutable records for events.
- Use immutable records for commands.
- Use immutable records for DTOs.
- Use `[Version]` on persisted types when evolving schemas.

### Don’t:
- Don’t put business logic in grains.
- Don’t throw exceptions for validation or business errors.
- Don’t use magic strings or numbers.
- Don’t use `[Alias]` on grain classes.
- Don’t use service locator patterns.

## 11. Onboarding Checklist

- Read this manifesto and review the solution structure.
- Run all tests locally before submitting code.
- Follow naming, structure, and error handling conventions.
- Ask for code reviews and be open to feedback.
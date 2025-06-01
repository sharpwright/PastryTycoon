# PastryTycoon

PastryTycoon is a modular, event-driven simulation game backend built with .NET and Orleans. It models the management of a virtual pastry business, supporting scalable state management and extensibility.

## Features

- **Orleans-based Grains:** Distributed, actor-based state management.
- **Event Sourcing:** Reliable, auditable state changes.
- **Validation:** Business logic validation using FluentValidation.
- **Azure Storage Support:** Local development with Azurite; ready for Azure deployment.
- **Modular Architecture:** Clear separation between core logic, abstractions, and data access.

## Project Structure

- `data/`  
  Local development data, such as Azurite storage files and emulator data.
- `docs/`
  Documentation files, architecture diagrams, and additional project resources.
- `src/`
  Main source code folder containing all projects.
  - `PastryTycoon.Core.Abstractions/`  
  Interfaces, contracts, and shared abstractions used across the solution.
  - `PastryTycoon.Core.Grains/`  
  Orleans grains and core business logic for the simulation.
  - `PastryTycoon.Core.Grains.UnitTests/`  
  Unit tests for the core grains and business logic.
  - `PastryTycoon.Data/`  
  Data access logic, storage providers, and persistence-related code.
  - `PastryTycoon.Data.UnitTests/`  
  Unit tests for the data access layer.
  - `PastryTycoon.Silo/`  
  Orleans Silo host project, responsible for running the Orleans cluster.  
  - `PastryTycoon.Web.API/`  
  ASP.NET Core Web API project for exposing endpoints to clients.
  - `PastryTycoon.sln`  
  Solution file for the project.
- `.gitignore`  
  Git ignore rules for the repository.
- `README.md`  
  Project documentation and instructions.

This structure helps organize the backend, API, data, and tests for maintainability and scalability.

## Getting Started

1. **Clone the repository:**

```
git clone https://github.com/your-org/PastryTycoon.git
```

2. **Install dependencies:**

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Azurite](https://marketplace.visualstudio.com/items?itemName=Azurite.azurite) (for local Azure Storage emulation)

3. **Run Azurite (for local development):**
- Use the VS Code Azurite extension or run:
  ```
  npx azurite
  ```

## Building and Running the Solution

1. **Build the Solution**

From the root of the repository, run:
```
dotnet build src/PastryTycoon.sln
```

2. **Run Unit Tests**

To execute all unit tests (for both core grains and data layers), run:
```
dotnet test src/PastryTycoon.sln
```

3. **Run the Orleans Silo**

To start the Orleans Silo host for local development:

```
dotnet run --project src/PastryTycoon.Silo
```

4. **Run the Web API**

To start the ASP.NET Core Web API:
```
dotnet run --project src/PastryTycoon.Web.API
```

> **Tip:** You can run the Silo and Web API in separate terminals to simulate a full backend environment.

## Development

- Use Visual Studio Code for best integration with Azurite and .NET tooling.
- Data files in the `data/` folder are for local development only and are excluded from version control.

## Contributing

Contributions are welcome! Please open issues or submit pull requests.

## License

This project is licensed under the MIT License.

## Acknowledgements

- [Microsoft Orleans](https://dotnet.github.io/orleans/)
- [Azurite](https://github.com/Azure/Azurite)
- [FluentValidation](https://fluentvalidation.net/)
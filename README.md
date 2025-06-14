# PastryTycoon

PastryTycoon is a modular, event-driven simulation game backend built with .NET and Orleans. It models the management of a virtual pastry business, supporting scalable state management and extensibility.

## Features

- **Orleans-based Grains:** Distributed, actor-based state management.
- **Event Sourcing:** Reliable, auditable state changes.
- **Validation:** Business logic validation using FluentValidation.
- **Azure Storage Support:** Local development with Azurite; ready for Azure deployment.
- **Modular Architecture:** Clear separation between core logic, abstractions, and data access.
- **Aspire Orchestration:** Unified local development experience using .NET Aspire for running all dependencies and services.
- **Comprehensive Testing:** Unit tests for both core grains and data access layers.
- **Developer Tooling:** Integrated dashboards for Orleans, Aspire, and storage emulators for easy monitoring and debugging.

## Project Structure

- `docs/`
  Documentation files, architecture diagrams, and additional project resources.
- `src/`
  Main source code folder containing all projects.
  - `PastryTycoon.AppHost/`  
    Aspire AppHost project for orchestrating local development dependencies and services.
  - `PastryTycoon.Core.Abstractions/`  
    Interfaces, contracts, and shared abstractions used across the solution.
  - `PastryTycoon.Core.Grains/`  
    Orleans grains and core business logic for the simulation.
  - `PastryTycoon.Core.Grains.IntegrationTests/`  
    Integration tests for testing event flows in the grains using the Orleans Testing Host.
  - `PastryTycoon.Core.Grains.UnitTests/`  
    Unit tests for the core grains and business logic using mocks and OrleansTestkit.
  - `PastryTycoon.Data/`  
    Data access logic, storage providers, and persistence-related code.
  - `PastryTycoon.Data.UnitTests/`  
    Unit tests for the data access layer.
  - `PastryTycoon.ServiceDefaults/`  
    Shared service configuration, OpenTelemetry, and cross-cutting concerns for all projects.
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
- [Docker](https://www.docker.com/products/docker-desktop) (for running Azurite and CosmosDB emulator via Aspire AppHost)
- (Optional) [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/) for browsing and managing Azurite/Azure Storage data

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

3. **Run the project**

To run all dependencies, the Orleans Silo and the Web API:
```
dotnet run --project src/PastryTycoon.AppHost
```

The following tools let you browse, manage, and debug your local development data:

- **Access Azurite:**  
  Use [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/) to connect to Azurite at `http://127.0.0.1:10000` (Blob), `http://127.0.0.1:10001` (Queue), and `http://127.0.0.1:10002` (Table).  
  In Storage Explorer, add a new connection using "Attach to a local emulator" and select Azurite.

- **Access CosmosDB Emulator:**  
  Use [Azure Cosmos DB Explorer](https://cosmos.azure.com/) or compatible tools.  
  Connect to the emulator at [https://localhost:8081/_explorer/index.html](https://localhost:8081/_explorer/index.html) (you may need to accept the self-signed certificate).  
  For connection strings and keys, see the [official Cosmos DB emulator documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/emulator).

- **Access Aspire Dashboard:**  
  After starting, open [http://localhost:18888](http://localhost:18888) in your browser to view the Aspire dashboard, which provides health, logs, and endpoint links for all running services.

- **Access Orleans Dashboard:**  
   After starting, open [http://localhost:8080](http://localhost:8080) in your browser to view the Orleans dashboard for cluster monitoring and management.  
  > **Note:** If you do not see a link to the Orleans dashboard in the Aspire dashboard, simply open the URL above directly in your browser.

## Development

1. **Developer manifesto**

> [!IMPORTANT]
> Read the [Developer Manifesto](docs/developer-manifesto.md) before writing and committing any code!

2. **Tooling**
   
For the best development experience with PastryTycoon in VSCode, install the following extensions:

- [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)  
  Core C# language support (required by C# Dev Kit).

- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)  
  Full-featured C# support for .NET projects, including IntelliSense, debugging, and project management.

- [IntelliCode for C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=VisualStudioExptTeam.vscodeintellicode-csharp)  
  AI-assisted code completions and recommendations for C# Dev Kit.

- [Container Tools](https://marketplace.visualstudio.com/items?itemName=ms-vscode.vscode-container-tools)  
  Enhanced support for developing with containers in VS Code.

- [Docker](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)  
  Build, run, and manage Docker containers and images.

- [Markdown All in One](https://marketplace.visualstudio.com/items?itemName=yzhang.markdown-all-in-one)  
  Enhanced Markdown editing and preview support.

- [Todo Tree](https://marketplace.visualstudio.com/items?itemName=Gruntfuggly.todo-tree)  
  Quickly search and visualize TODO comments and code annotations across your project.

## Contributing

Contributions are welcome! Please open issues or submit pull requests.

## License

This project is licensed under the MIT License.

## Acknowledgements

- [Aspire](https://github.com/dotnet/aspire)  
  .NET application composition and orchestration for local development.

- [Azurite](https://github.com/Azure/Azurite)  
  Local Azure Storage emulator for development and testing.

- [Cosmos DB Emulator](https://learn.microsoft.com/azure/cosmos-db/emulator)  
  Local emulator for Azure Cosmos DB, used for development and testing.

- [Docker](https://www.docker.com/)  
  Containerization platform used for running local dependencies.

- [FluentValidation](https://fluentvalidation.net/)  
  Validation library for .NET used for business rule enforcement.

- [Microsoft Orleans](https://dotnet.github.io/orleans/)  
  Distributed application framework powering the actor-based backend.

- [Visual Studio Code](https://code.visualstudio.com/)  
  Recommended editor for development with rich extension support.


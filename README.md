# PastryTycoon

PastryTycoon is a modular, event-driven simulation game backend built with .NET and Orleans. It models the management of a virtual pastry business, supporting scalable state management and extensibility.

## Features

- **Orleans-based Grains:** Distributed, actor-based state management.
- **Event Sourcing:** Reliable, auditable state changes.
- **Validation:** Business logic validation using FluentValidation.
- **Azure Storage Support:** Local development with Azurite; ready for Azure deployment.
- **Modular Architecture:** Clear separation between core logic, abstractions, and data access.

## Project Structure

- `docker-compose/`
  Docker Compose files and related resources for running development dependencies (e.g., Azurite, CosmosDB emulator).
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

2. **Install dependencies:**

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Docker](https://www.docker.com/products/docker-desktop) (for running Azurite and CosmosDB emulator via Docker Compose)
- (Optional) [VS Code Azurite extension](https://marketplace.visualstudio.com/items?itemName=Azurite.azurite) for local Azure Storage emulation
- (Optional) [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/) for browsing and managing Azurite/Azure Storage data

1. **Start development dependencies:**

From the root of the repository, run:

```
docker-compose -f docker-compose/docker-compose.yml up -d
```

This will start Azurite and the CosmosDB emulator in containers. The following tools let you browse, manage, and debug your local development data.

- **Access Azurite:**  
  Use [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/) to connect to Azurite at `http://127.0.0.1:10000` (Blob), `http://127.0.0.1:10001` (Queue), and `http://127.0.0.1:10002` (Table).  
  In Storage Explorer, add a new connection using "Attach to a local emulator" and select Azurite.

- **Access CosmosDB Emulator:**  
    Connect to the emulator at `https://localhost:8081/_explorer/index.html` (you may need to accept the self-signed certificate).  
  For connection strings and keys, see the [official Cosmos DB emulator documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/emulator).

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
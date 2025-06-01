# Game Event Flows

Legend:

- **event**: Raised and persisted via event sourcing.
- **stream**: Published to an Orleans stream.
- **direct call**: Grain-to-grain method invocation.

## Creating a New Game

| Step | Actor/Grain         | Action/Method                  | Event Raised / Published          | Next Handler(s) / Subscriber(s)       | Notes                                                            |
| ---- | ------------------- | ------------------------------ | --------------------------------- | ------------------------------------- | ---------------------------------------------------------------- |
| 1    | GameFactoryGrain    | CreateNewGameAsync             | -                                 | GameGrain, PlayerGrain                | Validates input, generates IDs, orchestrates initialization      |
| 2    | PlayerGrain         | InitializeAsync                | PlayerInitializedEvent (event)    | -                                     | Sets up player state for new game                                |
| 3    | GameGrain           | InitializeGameStateAsync       | GameCreatedEvent (event + stream) | GameProjectionGrain (implicit stream) | Initializes game state, persists and streams creation event      |
| 4    | GameProjectionGrain | OnNextAsync (GameCreatedEvent) | -                                 | -                                     | Updates projections, logs, or triggers further actions as needed |

---

## Game Initialization

| Step | Actor/Grain | Action/Method              | Event Raised / Published                | Next Handler(s) / Subscriber(s)           | Notes                                                        |
| ---- | ----------- | -------------------------- | --------------------------------------- | ----------------------------------------- | ------------------------------------------------------------ |
| 1    | GameGrain   | InitializeGameStateAsync   | GameStateInitializedEvent (event + stream) | GameProjectionGrain (implicit stream) | Validates input, raises and persists event, publishes to stream |

---

## Game Update

| Step | Actor/Grain | Action/Method              | Event Raised / Published                | Next Handler(s) / Subscriber(s)           | Notes                                                        |
| ---- | ----------- | -------------------------- | --------------------------------------- | ----------------------------------------- | ------------------------------------------------------------ |
| 1    | GameGrain   | UpdateGameAsync            | GameUpdatedEvent (event + stream)       | GameProjectionGrain (implicit stream)     | Validates input, raises and persists event, publishes to stream |

---

## Game Statistics Query

| Step | Actor/Grain | Action/Method              | Event Raised / Published | Next Handler(s) / Subscriber(s) | Notes                                  |
| ---- | ----------- | -------------------------- | ------------------------ | ------------------------------- | -------------------------------------- |
| 1    | GameGrain   | GetGameStatisticsAsync     | -                        | -                               | Returns current game statistics        |


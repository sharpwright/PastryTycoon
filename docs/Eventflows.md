# BakerySim Event Flow Table

Legend:

- **event**: Raised and persisted via event sourcing.
- **stream**: Published to an Orleans stream.
- **direct call**: Grain-to-grain method invocation.

## Game

### Game starts

| Step | Actor/Grain         | Action/Method                  | Event Raised / Published          | Next Handler(s) / Subscriber(s)       | Notes                                                            |
| ---- | ------------------- | ------------------------------ | --------------------------------- | ------------------------------------- | ---------------------------------------------------------------- |
| 1    | GameGrain           | StartGame                      | GameStartedEvent (event + stream) | GameProjectionGrain (implicit stream) | Initializes game state, validates input                          |
| 2    | GameProjectionGrain | OnNextAsync (GameStartedEvent) | -                                 | -                                     | Updates projections, logs, or triggers further actions as needed |

## Player

### Player discovers recipe

| Step | Actor/Grain      | Action/Method                       | Event Raised / Published               | Next Handler(s) / Subscriber(s)      | Notes                              |
| ---- | ---------------- | ----------------------------------- | -------------------------------------- | ------------------------------------ | ---------------------------------- |
| 1    | PlayerGrain      | DiscoverRecipe                      | RecipeDiscoveredEvent (event + stream) | AchievementGrain (implicit stream)   | Validates recipe not already known |
| 2    | AchievementGrain | OnNextAsync (RecipeDiscoveredEvent) | AchievementUnlockedEvent (direct call) | PlayerGrain (UnlockAchievementAsync) | Only if achievement criteria met   |
| 3    | PlayerGrain      | UnlockAchievementAsync              | AchievementUnlockedEvent (event)       | -                                    | Updates player state               |


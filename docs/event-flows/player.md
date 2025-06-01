# Player Event Flows

Legend:

- **event**: Raised and persisted via event sourcing.
- **stream**: Published to an Orleans stream.
- **direct call**: Grain-to-grain method invocation.

## Player Initialization

| Step | Actor/Grain   | Action/Method         | Event Raised / Published         | Next Handler(s) / Subscriber(s) | Notes                                      |
| ---- | ------------- | --------------------- | -------------------------------- | ------------------------------- | ------------------------------------------ |
| 1    | PlayerGrain   | InitializeAsync       | PlayerInitializedEvent (event)   | -                               | Validates input, sets up player state      |

---

## Recipe Discovery

| Step | Actor/Grain   | Action/Method         | Event Raised / Published             | Next Handler(s) / Subscriber(s)   | Notes                                               |
| ---- | ------------- | --------------------- | ------------------------------------ | --------------------------------- | --------------------------------------------------- |
| 1    | PlayerGrain   | DiscoverRecipeAsync   | PlayerDiscoveredRecipeEvent (event)  | AchievementsGrain (stream)        | Persists event, publishes to player event stream    |
| 2    | AchievementsGrain | OnNextAsync (PlayerDiscoveredRecipeEvent) | - | PlayerGrain (UnlockAchievementAsync) | Updates achievements state, may unlock achievements |

---

## Achievement Unlock

| Step | Actor/Grain   | Action/Method         | Event Raised / Published                | Next Handler(s) / Subscriber(s) | Notes                                      |
| ---- | ------------- | --------------------- | --------------------------------------- | ------------------------------- | ------------------------------------------ |
| 1    | PlayerGrain   | UnlockAchievementAsync| PlayerUnlockedAchievementEvent (event)  | -                               | Persists achievement unlock in player state|

---

## Player Statistics Query

| Step | Actor/Grain   | Action/Method             | Event Raised / Published | Next Handler(s) / Subscriber(s) | Notes                                  |
| ---- | ------------- | ------------------------- | ------------------------ | ------------------------------- | -------------------------------------- |
| 1    | PlayerGrain   | GetPlayerStatisticsAsync  | -                        | -                               | Returns current player statistics      |

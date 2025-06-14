using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PastryTycoon.Core.Abstractions.Game;

namespace PastryTycoon.Web.API.Game
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        public GameController(IClusterClient clusterClient)
        {
            ClusterClient = clusterClient;
        }

        public IClusterClient ClusterClient { get; }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> Get(Guid gameId)
        {            
            var gameGrain = ClusterClient.GetGrain<IGameGrain>(gameId);
            var stats = await gameGrain.GetGameStatisticsAsync();
            return Ok(stats);
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            // TODO: Get a unique identifier for player (from authentication or session)
            // For now, we will use a hardcoded playerId for testing.
            var playerId = Guid.Parse("ac6db42a-c53d-49c6-ab54-53a29d2dc13a");
            var command = new CreateNewGameCmd(playerId, request.playerName,
                Enum.Parse<Core.Abstractions.Game.DifficultyLevel>(request.difficultyLevel.ToString()));

            // Create a new game ID and start the game
            var gameFactoryGrain = ClusterClient.GetGrain<IGameFactoryGrain>(Guid.Empty);
            var newGameId = await gameFactoryGrain.CreateNewGameAsync(command);

            return Ok(newGameId);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateGame([FromBody] UpdateGameRequest request)
        {
            if (request == null || request.GameId == Guid.Empty)
            {
                return BadRequest("Invalid request");
            }

            // Update the game with the provided game ID
            var gameGrain = ClusterClient.GetGrain<IGameGrain>(request.GameId);
            var gameCmd = new UpdateGameCmd(request.GameId, DateTime.UtcNow);
            await gameGrain.UpdateGameAsync(gameCmd);

            return Ok();
        }

    }
}

using BakerySim.Grains.Actors;
using BakerySim.Grains.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans.Runtime.GrainDirectory;

namespace BakerySim.Web.API.GameManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameManagerController : ControllerBase
    {
        public GameManagerController(IClusterClient clusterClient)
        {
            ClusterClient = clusterClient;
        }

        public IClusterClient ClusterClient { get; }

        [HttpGet("{gameId}")]
        public IActionResult Get(Guid gameId)
        {            
            var gameGrain = ClusterClient.GetGrain<IGameGrain>(gameId);
            return Ok();
        }

        [HttpPut("/start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            // TODO: Get playerId from access token (or similar). 
            // For now, we will use a hardcoded playerId for testing.
            var playerId = Guid.Parse("ac6db42a-c53d-49c6-ab54-53a29d2dc13a");

            // Create a new game ID and start the game
            var gameFactoryGrain = ClusterClient.GetGrain<IGameFactoryGrain>(Guid.Empty);
            var newGameId = await gameFactoryGrain.CreateNewGameAsync(playerId, request.GameName);

            return Ok(newGameId);
        }

        [HttpPost("/update")]
        public async Task<IActionResult> UpdateGame([FromBody] UpdateGameRequest request)
        {
            if (request == null || request.GameId == Guid.Empty)
            {
                return BadRequest("Invalid request");
            }

            // Update the game with the provided game ID
            var gameGrain = ClusterClient.GetGrain<IGameGrain>(request.GameId);
            var gameCmd = new UpdateGameCommand(request.GameId, request.GameName, DateTime.UtcNow);
            await gameGrain.UpdateGame(gameCmd);

            return Ok();
        }

    }
}

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

            // Create a new game ID and start the game
            var gameId = Guid.NewGuid();
            var gameGrain = ClusterClient.GetGrain<IGameGrain>(gameId);
            var gameCmd = new StartGameCommand(gameId, request.GameName, DateTime.UtcNow);
            await gameGrain.StartGame(gameCmd);

            return Ok();
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

using BakerySim.Grains.Actors;
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

    }
}

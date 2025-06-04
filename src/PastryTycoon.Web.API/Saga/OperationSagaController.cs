using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PastryTycoon.Core.Abstractions.Saga;

namespace PastryTycoon.Web.API.Saga
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationSagaController : ControllerBase
    {
        private readonly IClusterClient client;
        private readonly ILogger<OperationSagaController> logger;

        public OperationSagaController(IClusterClient client, ILogger<OperationSagaController> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        [HttpPost("SaveOperation")]
        public async Task<IActionResult> SaveOperation()
        {
            this.logger.LogInformation("Saving operation using saga.");

            var grain = client.GetGrain<IOperationSagaGrain>(Guid.NewGuid());
            var command = new SaveOperationCommand(
                Guid.NewGuid(),
                "Sample Operation",
                new List<string>
                {
                    "Activity 1",
                    "Activity 2",
                    "Activity 3"
                }
            );

            await grain.SaveOperation(command);

            this.logger.LogInformation("SaveOperationSaga is running.");

            return Ok();
        }
    }
}

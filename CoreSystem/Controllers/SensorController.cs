using CoreSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ILogger<SensorController> _logger;

        public SensorController(ILogger<SensorController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] SensorPayload payload)
        {
            _logger.LogInformation("Daten empfangen von {Device} um {Time}", payload.DeviceId, payload.Timestamp);

            if (payload.RawStatus.TryGetProperty("sensor", out var sensor) &&
                sensor.GetProperty("state").GetString() == "open")
            {
                _logger.LogWarning("!!! Tür geöffnet bei {Device} !!!", payload.DeviceId);
            }

            return Ok();
        }
    }
}

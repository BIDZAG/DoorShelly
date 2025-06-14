using System;
using System.Text.Json;

namespace CoreSystem.Models
{
    public class SensorPayload
    {
        public DateTime Timestamp { get; set; }
        public string DeviceId { get; set; } = null!;
        public JsonElement RawStatus { get; set; }
    }
}

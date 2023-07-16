using Newtonsoft.Json;

namespace GrowOps.Models
{
    public class TwinPatch
    {
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }
    // TwinPatch myDeserializedClass = JsonConvert.DeserializeObject<TwinPatch>(myJsonResponse);
    public class Desired
    {
        [JsonProperty("telemetry_interval")]
        public int TelemetryInterval { get; set; }
    }

    public class Properties
    {
        [JsonProperty("desired")]
        public Desired Desired { get; set; }
    }
}

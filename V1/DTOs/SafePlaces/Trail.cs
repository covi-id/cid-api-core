using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.SafePlaces
{
    public class Trail
    {
        [JsonProperty("time")]
        public long Time { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}

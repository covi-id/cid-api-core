using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.SafePlaces
{
    public class LoginResponse
    {
        public string Token { get; set; }
        [JsonProperty("maps_api_key")]
        public string MapsApiKey { get; set; }
    }
}

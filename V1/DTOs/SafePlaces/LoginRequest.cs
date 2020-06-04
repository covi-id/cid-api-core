using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.SafePlaces
{
    public class LoginRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}

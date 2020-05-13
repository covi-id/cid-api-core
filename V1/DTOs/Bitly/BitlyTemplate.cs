using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.Bitly
{
    public class BitlyTemplate
    {
        [JsonProperty("long_url")] public string Url { get; set; }
    }
}
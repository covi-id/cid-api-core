using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.Clickatell
{
    public class ClickatellResponse
    {
        [JsonProperty("balance")] public double Balance { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }
    }
}
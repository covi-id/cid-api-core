using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.Organisation
{
    public class UpdateCountRequest
    {
        public string WalletId { get; set; }
        [JsonProperty("long")] public decimal Longitude { get; set; }
        [JsonProperty("lat")] public decimal Latitude { get; set; }
    }

    public class UpdateCountResponse
    {
        public int Balance { get; set; }
    }
}
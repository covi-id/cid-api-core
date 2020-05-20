using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.Organisation
{
    public class UpdateCountRequest
    {
        [Required]
        public string WalletId { get; set; }
        [JsonProperty("long")] public decimal Longitude { get; set; }
        [JsonProperty("lat")] public decimal Latitude { get; set; }
    }

    public class UpdateCountResponse
    {
        public int Balance { get; set; }
        public int Total { get; set; }
    }
}
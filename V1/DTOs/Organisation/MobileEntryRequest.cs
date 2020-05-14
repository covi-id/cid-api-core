using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CoviIDApiCore.V1.DTOs.Organisation
{
    public class MobileEntryRequest
    {
        [StringLength(16, MinimumLength = 9, ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; }
        [JsonProperty("long")] 
        public decimal Longitude { get; set; }
        [JsonProperty("lat")] 
        public decimal Latitude { get; set; }
    }
}

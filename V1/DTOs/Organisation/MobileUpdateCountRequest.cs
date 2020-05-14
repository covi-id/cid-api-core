using CoviIDApiCore.V1.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CoviIDApiCore.V1.DTOs.Organisation
{
    public class MobileUpdateCountRequest
    {
        [StringLength(16, MinimumLength = 9, ErrorMessage = "Invalid mobile number")]
        [Encrypted(true)]
        public string MobileNumber { get; set; }
        [JsonProperty("long")] 
        public decimal Longitude { get; set; }
        [JsonProperty("lat")] 
        public decimal Latitude { get; set; }
    }
}

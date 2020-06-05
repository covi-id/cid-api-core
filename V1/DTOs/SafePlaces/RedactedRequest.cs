using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoviIDApiCore.V1.DTOs.SafePlaces
{
    public class RedactedRequest
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        [JsonProperty("trail")]
        public List<Trail> Trails { get; set; }
    }
}

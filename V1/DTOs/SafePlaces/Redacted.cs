using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoviIDApiCore.V1.DTOs.SafePlaces
{
    public class Redacted
    {
        public int Identifier { get; set; }
        [JsonProperty("organization_id")]
        public int OrganizationId { get; set; }
        public List<Trail> Trails { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}

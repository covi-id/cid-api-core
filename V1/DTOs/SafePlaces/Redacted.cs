using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace CoviIDApiCore.V1.DTOs.SafePlaces
{
    public class Redacted
    {
        [JsonProperty("identifier")]
        public int Identifier { get; set; }
        [JsonProperty("organization_id")]
        public int OrganizationId { get; set; }
        [JsonProperty("trail")]
        public List<Trail> Trails { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
    
    public class BaseResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}

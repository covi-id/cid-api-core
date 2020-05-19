using System.Collections.Generic;

namespace CoviIDApiCore.V1.DTOs.SafePlaces
{
    public class RedactedRequest
    {
        public string Identifier { get; set; }
        public List<Trail> Trails { get; set; }
    }
}

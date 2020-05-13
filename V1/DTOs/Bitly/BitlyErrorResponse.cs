using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.Bitly
{
    public class BitlyErrorResponse
    {
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("errors")] public Error[] Errors { get; set; }
        [JsonProperty("resource")] public string Resource { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
    }

    public class Error
    {
        [JsonProperty("field")] public string Field { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("error_code")] public string ErrorCode { get; set; }
    }
}
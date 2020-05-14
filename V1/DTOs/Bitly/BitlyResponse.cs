using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.Bitly
{
    public class BitlyResponse
    {
        [JsonProperty("references")] public object References { get; set; }
        [JsonProperty("archived")] public bool Archived { get; set; }
        [JsonProperty("tags")] public string[] Tags { get; set; }
        [JsonProperty("created_at")] public string CreatedAt { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("deeplinks")] public DeepLink[] Deeplinks { get; set; }
        [JsonProperty("created_by")] public string CreatedBy { get; set; }
        [JsonProperty("long_url")] public string LongUrl { get; set; }
        [JsonProperty("client_id")] public string ClientId { get; set; }
        [JsonProperty("custom_bitlinks")] public string[] CustomBitLinks { get; set; }
        [JsonProperty("link")] public string Link { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
    }

    public class DeepLink
    {
        [JsonProperty("bitlink")] public string BitLink { get; set; }
        [JsonProperty("install_url")] public string InstallUrl { get; set; }
        [JsonProperty("created")] public string Created { get; set; }
        [JsonProperty("app_uri_path")] public string ApiUriPath { get; set; }
        [JsonProperty("modified")] public string Modified { get; set; }
        [JsonProperty("install_type")] public string InstallType { get; set; }
        [JsonProperty("app_guid")] public string AppGuid { get; set; }
        [JsonProperty("guid")] public string Guid { get; set; }
        [JsonProperty("os")] public string Os { get; set; }
        [JsonProperty("brand_guid")] public string BrandGuid { get; set; }
    }
}
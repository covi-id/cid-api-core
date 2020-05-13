using CoviIDApiCore.V1.DTOs.Wallet;
using Newtonsoft.Json;

namespace CoviIDApiCore.V1.DTOs.Organisation
{
    public class MobileEntryRequest
    {
        public CreateWalletRequest WalletRequest { get; set; }
        [JsonProperty("long")] 
        public decimal Longitude { get; set; }
        [JsonProperty("lat")] 
        public decimal Latitude { get; set; }
    }
}

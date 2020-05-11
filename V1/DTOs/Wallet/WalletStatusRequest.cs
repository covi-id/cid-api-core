using System.ComponentModel.DataAnnotations;

namespace CoviIDApiCore.V1.DTOs.Wallet
{
    public class WalletStatusRequest
    {
        [Required]
        public string Key { get; set; }
    }
}
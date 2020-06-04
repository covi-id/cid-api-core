using System.ComponentModel.DataAnnotations;

namespace CoviIDApiCore.V1.DTOs.Wallet
{
    public class CreateWalletRequest
    {
        [StringLength(16, MinimumLength = 9, ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; }
    }
}

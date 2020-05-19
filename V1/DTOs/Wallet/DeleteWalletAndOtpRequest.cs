using CoviIDApiCore.V1.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CoviIDApiCore.V1.DTOs.Wallet
{
    public class DeleteWalletAndOtpRequest
    {
        [Encrypted(true)]
        [StringLength(16, MinimumLength = 9, ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; }
    }
}

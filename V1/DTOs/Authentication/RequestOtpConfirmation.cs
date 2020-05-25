using System.ComponentModel.DataAnnotations;
using CoviIDApiCore.V1.DTOs.Wallet;

namespace CoviIDApiCore.V1.DTOs.Authentication
{
    public class RequestOtpConfirmation
    {
        [Required]
        public int Otp { get; set; }
        public WalletDetailsRequest WalletDetails { get; set; }
    }

    public class WalletDetailsRequest
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Invalid length. Minimum length is 2 and maximum is 50")]
        public string FirstName { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Invalid length. Minimum length is 2 and maximum is 50")]
        public string LastName { get; set; }
        public string Photo { get; set; }
        public IdType IdType { get; set; }
        [StringLength(13, MinimumLength = 6, ErrorMessage = "Invalid length. Minimum length is 6 and maximum is 13")]
        public string IdValue { get; set; }
    }
}
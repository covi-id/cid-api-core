using System;
using System.ComponentModel.DataAnnotations;

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
        //todo validation
        public string MobileNumber { get; set; }
        public bool isMyMobileNumber { get; set; }
        public string Photo { get; set; }
        public bool HasConsent { get; set; }
    }
}
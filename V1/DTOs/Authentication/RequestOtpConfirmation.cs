using System.ComponentModel.DataAnnotations;
using CoviIDApiCore.V1.DTOs.WalletTestResult;

namespace CoviIDApiCore.V1.DTOs.Authentication
{
    public class RequestOtpConfirmation
    {
        [Required]
        public int Otp { get; set; }
        public TestResultRequest TestResult { get; set; }
        public WalletDetailsRequest WalletDetails { get; set; }

        public bool isValid()
        {
            return TestResult.isValid();
        }
    }

    public class WalletDetailsRequest
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Invalid length. Minimum length is 2 and maximum is 50")]
        public string FirstName { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Invalid length. Minimum length is 2 and maximum is 50")]
        public string LastName { get; set; }
        public string Photo { get; set; }
    }
}
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IOtpService
    {
        Task<long> GenerateAndSendOtpAsync(string mobileNumber);
        Task<TokenResponse> ResendOtpAsync(RequestResendOtp payload, string authToken);
        Task<OtpConfirmationResponse> ConfirmOtpAsync(RequestOtpConfirmation payload, string authToken);
        Task ConfirmDeleteWallet(OtpDeleteWalletRequest request, string authToken);
    }
}
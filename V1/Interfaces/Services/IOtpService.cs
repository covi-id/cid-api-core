using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IOtpService
    {
        Task<string> GenerateAndSendOtpAsync(string mobileNumber);
        Task ResendOtp(RequestResendOtp payload);
        Task<OtpConfirmationResponse> ConfirmOtpAsync(RequestOtpConfirmation payload);
    }
}
using CoviIDApiCore.V1.DTOs.Wallet;
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.Models.Database;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IWalletService
    {
        Task<WalletStatusResponse> GetWalletStatus(string walletId, string key);
        Task<TokenResponse> CreateWalletAndOtp(CreateWalletRequest walletRequest, string sessionId = null);
        Task<Wallet> GetWalletByMobileNumebr(string mobileNumber);
        Task<Wallet> CreateMobileWallet(CreateWalletRequest request, string organisationName);
        Task DeleteWallet(string walletId);
        Task<Wallet> UpdateWalletToVerified(string walletId);
    }
}

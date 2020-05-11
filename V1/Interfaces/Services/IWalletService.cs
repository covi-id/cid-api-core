using CoviIDApiCore.V1.DTOs.Wallet;
using System;
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IWalletService
    {
        Task<WalletStatusResponse> GetWalletStatus(string walletId, string key);
        Task<TokenResponse> CreateWallet(CreateWalletRequest walletRequest);
    }
}

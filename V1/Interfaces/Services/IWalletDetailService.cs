using System.Collections.Generic;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IWalletDetailService
    {
        Task<WalletDetail> CreateWalletDetails(Wallet wallet, WalletDetailsRequest walletDetails, string key);
        Task<WalletDetail> CreateMobileWalletDetails(Wallet wallet, string mobileNumber);
        Task<List<WalletDetail>> GetWalletDetailsByMobileNumber(string mobileNumber);
        Task DeleteWalletDetails(Wallet wallet);
    }
}
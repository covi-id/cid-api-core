using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IWalletDetailService
    {
        Task AddWalletDetails(Wallet wallet, WalletDetailsRequest walletDetails, string key);
        Task DeleteWalletDetails(Wallet wallet);
    }
}
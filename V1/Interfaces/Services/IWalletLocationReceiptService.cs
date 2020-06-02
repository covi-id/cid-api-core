using CoviIDApiCore.Models.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IWalletLocationReceiptService
    {
        Task<WalletLocationReceipt> CreateReceipt(Wallet wallet, decimal longitude, decimal latitude, ScanType scanType);
        Task<List<WalletLocationReceipt>> GetReceiptsForToday(Wallet wallet);
    }
}

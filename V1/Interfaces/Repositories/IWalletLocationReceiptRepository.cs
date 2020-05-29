using CoviIDApiCore.Models.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface IWalletLocationReceiptRepository : IBaseRepository<WalletLocationReceipt, Guid>
    {
        Task<List<WalletLocationReceipt>> GetReceiptsForTodayByWallet(Wallet wallet);
    }
}

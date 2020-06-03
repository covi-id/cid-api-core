using CoviIDApiCore.Models.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface IWalletDetailRepository : IBaseRepository<WalletDetail, Guid>
    {
        Task<List<WalletDetail>> GetWalletDetailsByWallet(Wallet wallet);
        Task<List<WalletDetail>> GeWalletDetailstByEncryptedMobileNumber(string encryptedMobileNumber);
    }
}

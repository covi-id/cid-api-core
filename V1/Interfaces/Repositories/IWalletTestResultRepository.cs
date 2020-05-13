using CoviIDApiCore.Models.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface IWalletTestResultRepository : IBaseRepository<WalletTestResult, Guid>
    {
        Task<List<WalletTestResult>> GetTestResults(Guid walletId);
    }
}

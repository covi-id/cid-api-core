using CoviIDApiCore.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface ICovidTestRepository : IBaseRepository<WalletTestResult, Guid>
    {
    }
}

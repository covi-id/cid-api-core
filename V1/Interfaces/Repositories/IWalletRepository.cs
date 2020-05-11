using System;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface IWalletRepository: IBaseRepository<Wallet, Guid>
    {
    }
}
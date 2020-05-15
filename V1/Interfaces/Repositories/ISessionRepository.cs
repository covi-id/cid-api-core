using CoviIDApiCore.Models.Database;
using System;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface ISessionRepository : IBaseRepository<Session, Guid>
    {
        Task<Session> GetSessionAndWalletAsync(Guid sessionId);
    }
}

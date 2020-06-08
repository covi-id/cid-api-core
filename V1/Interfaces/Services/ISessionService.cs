using CoviIDApiCore.Models.Database;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ISessionService
    {
        Task<Session> CreateSession(Wallet wallet);
        Task<Session> GetAndUseSession(string sessionId);
    }
}

using CoviIDApiCore.Models.Database;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ISessionService
    {
        Task<Session> CreateSession(string mobileNumber, Wallet wallet);
        Task<Session> GetAndUseSession(string sessionId);
    }
}

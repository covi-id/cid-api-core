using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Session;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ISessionService
    {
        Task<Session> CreateSession(CreateSessionRequest createSessionRequest);
    }
}

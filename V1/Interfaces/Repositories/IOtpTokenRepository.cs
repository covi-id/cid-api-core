 using System.Collections.Generic;
 using System.Threading.Tasks;
 using CoviIDApiCore.Models.Database;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface IOtpTokenRepository : IBaseRepository<OtpToken, long>
    {
        Task<OtpToken> GetUnusedByMobileNumber(string mobileNumber);
        Task<OtpToken> GetBySessionId(string sessionId);
        Task<List<OtpToken>> GetAllUnexpiredByMobileNumberAsync(string mobileNumber);
    }
}

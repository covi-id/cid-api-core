 using System.Collections.Generic;
 using System.Threading.Tasks;
 using CoviIDApiCore.Models.Database;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface IOtpTokenRepository : IBaseRepository<OtpToken, long>
    {
        Task<List<OtpToken>> GetAllUnexpiredByMobileNumber(string mobileNumber);
    }
}

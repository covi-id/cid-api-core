 using System;
 using System.Threading.Tasks;
 using CoviIDApiCore.Models.Database;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface IOrganisationRepository : IBaseRepository<Organisation, Guid>
    {
        Task<Organisation> GetWithLogsAsync(Guid id);
    }
}

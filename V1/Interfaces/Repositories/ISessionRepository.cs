using CoviIDApiCore.Models.Database;
using System;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface ISessionRepository : IBaseRepository<Session, Guid>
    {
    }
}

using CoviIDApiCore.Models.Database;
using System;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    interface ISessionRepository : IBaseRepository<Session, Guid>
    {
    }
}

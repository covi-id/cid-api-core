﻿ using System;
 using System.Collections.Generic;
 using System.Threading.Tasks;
 using CoviIDApiCore.Models.Database;

namespace CoviIDApiCore.V1.Interfaces.Repositories
{
    public interface IOrganisationAccessLogRepository : IBaseRepository<OrganisationAccessLog, Guid>
    {
        Task<List<OrganisationAccessLog>> GetAllCurrentDayByOrganisation(Organisation organisation);
    }
}

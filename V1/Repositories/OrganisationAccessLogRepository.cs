using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;

namespace CoviIDApiCore.V1.Repositories
{
    public class OrganisationAccessLogRepository : BaseRepository<OrganisationAccessLog, Guid>, IOrganisationAccessLogRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<OrganisationAccessLog> _dbSet;

        public OrganisationAccessLogRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.OrganisationAccessLogs;
        }

        public async Task<List<OrganisationAccessLog>> GetByCurrentDayByOrganisation(Organisation organisation)
        {
            return await _dbSet
                .Where(oal => Equals(oal.Organisation, organisation))
                .Where(oal => oal.CreatedAt.Value.Date.Equals(DateTime.UtcNow.Date))
                .ToListAsync();
        }

        public async Task<List<OrganisationAccessLog>> GetLogsForLastTwoWeeks(Guid walletId, DateTime testedAt)
        {
            return await _dbSet.Where(oal => Equals(oal.Wallet.Id, walletId))
                  .Where(oal => oal.CreatedAt >= testedAt.AddDays(-14) && oal.ScanType == ScanType.CheckIn)
                  .ToListAsync();

        }
    }
}

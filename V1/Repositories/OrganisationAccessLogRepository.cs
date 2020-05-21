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

        public async Task<List<OrganisationAccessLog>> GetListByWalletIds(List<Guid> walletIds)
        {
            return await _dbSet
                .Where(oal => walletIds.Contains(oal.Wallet.Id))
                .Where(oal => oal.CreatedAt.Value.Date == DateTime.UtcNow.Date)
                .Include(oal => oal.Wallet)
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;

namespace CoviIDApiCore.V1.Repositories
{
    public class OrganisationRepository : BaseRepository<Organisation, Guid>, IOrganisationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Organisation> _dbSet;

        public OrganisationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Organisations;
        }

        public async Task<Organisation> GetWithLogsAsync(Guid id)
        {
            return await _dbSet
                .Where(o => o.Id == id)
                .Include(o => o.AccessLogs)
                .FirstOrDefaultAsync();
        }
    }
}

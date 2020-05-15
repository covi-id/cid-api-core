using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Repositories
{
    public class SessionRepository : BaseRepository<Session, Guid>, ISessionRepository
    {
        private ApplicationDbContext _context;
        private DbSet<Session> _dbset;

        public SessionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbset = context.Sessions;
        }

        public async Task<Session> GetSessionAndWalletAsync(Guid sessionId)
        {
            return await _dbset
                .Where(s => Equals(s.Id, sessionId))
                .Include(s => s.Wallet)
                .SingleOrDefaultAsync();
        }
    }
}

using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoviIDApiCore.V1.Repositories
{
    public class SessionRepository : BaseRepository<Session, Guid>, ISessionRepository
    {
        private ApplicationDbContext _context;
        private DbSet<Session> _dbset;
        public SessionRepository(ApplicationDbContext context) : base(context)
        {
            context = _context;
            _dbset = _context.Sessions;
        }
    }
}

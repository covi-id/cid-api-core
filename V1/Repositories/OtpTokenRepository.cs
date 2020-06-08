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
    public class OtpTokenRepository : BaseRepository<OtpToken, long>, IOtpTokenRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<OtpToken> _dbSet;

        public OtpTokenRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.OtpTokens;
        }

        public async Task<List<OtpToken>> GetAllUnexpiredByMobileNumber(string mobileNumber)
        {
            return await _dbSet
                .Where(t => t.MobileNumber == mobileNumber)
                .Where(t => !t.isUsed)
                .Where(t => t.ExpireAt < DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}

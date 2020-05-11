using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;

namespace CoviIDApiCore.V1.Repositories
{
    public class WalletRepository : BaseRepository<Wallet, Guid>, IWalletRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Wallet> _dbSet;

        public WalletRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Wallets;
        }

        public async Task<Wallet> GetByWalletIdentifier(string identifier)
        {
            return await _dbSet
                .Where(t => string.Equals(t.Id, identifier))
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Wallet> GetByMobileNumber(string mobileNumber)
        {
            return await _dbSet
                .Where(t => string.Equals(t.MobileNumber, mobileNumber))
                .FirstOrDefaultAsync();
        }
    }
}

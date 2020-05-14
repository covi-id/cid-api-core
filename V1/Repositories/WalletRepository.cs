using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<Wallet> GetWallet(string mobileNumber)
        {
            var wallet = await _dbSet.FirstOrDefaultAsync(w => string.Equals(w.MobileNumber, mobileNumber, StringComparison.Ordinal));
            return wallet;
        }

    }
}

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

        //TODO: Change this once indicator is available in the model
        public async Task<Wallet> GetWalletByMobileNumberAndIndicator(string mobileNumber)
        {
            return await _dbSet
                .Where(w => string.Equals(w.MobileNumber, mobileNumber))
//                .Where(w => w.Indicator)
                .FirstOrDefaultAsync();
        }
    }
}

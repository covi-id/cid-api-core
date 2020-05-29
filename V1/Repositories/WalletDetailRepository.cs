using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Repositories
{
    public class WalletDetailRepository : BaseRepository<WalletDetail, Guid>, IWalletDetailRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<WalletDetail> _dbSet;

        public WalletDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.WalletDetails;
        }

        public async Task<List<WalletDetail>> GetWalletDetailsByWallet(Wallet wallet)
        {
            return await _dbSet
                .Where(wd => Equals(wd.Wallet, wallet))
                .ToListAsync();
        }

        public async Task<List<WalletDetail>> GetByEncryptedMobileNumber(string encryptedMobileNumber)
        {
            return await _dbSet
                .Where(w => string.Equals(w.MobileNumber, encryptedMobileNumber))
                .ToListAsync();
        }
    }
}

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
    public class WalletLocationReceiptRepository : BaseRepository<WalletLocationReceipt, Guid>, IWalletLocationReceiptRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<WalletLocationReceipt> _dbSet;

        public WalletLocationReceiptRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.WalletLocationReceipts;
        }

        public async Task<List<WalletLocationReceipt>> GetReceiptsForDate(Wallet wallet, DateTime forDate)
        {
            return await _dbSet
                .Where(r => r.Wallet == wallet)
                .Where(r => r.CreatedAt.Value.Date == forDate)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<WalletLocationReceipt>> GetReceiptsByStartDate(Guid walletId, DateTime startDate)
        {
            return await _dbSet.Where(oal => Equals(oal.Wallet.Id, walletId))
                  .Where(oal => oal.CreatedAt >= startDate && oal.ScanType == ScanType.CheckIn)
                  .ToListAsync();

        }
    }
}

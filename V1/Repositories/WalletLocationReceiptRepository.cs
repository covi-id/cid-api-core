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

        public async Task<List<WalletLocationReceipt>> GetReceiptsForTodayByWallet(Wallet wallet)
        {
            return await _dbSet
                .Where(r => r.Wallet == wallet)
                .Where(r => r.CreatedAt.Value.Date == DateTime.Now.Date)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}

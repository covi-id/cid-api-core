using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoviIDApiCore.V1.Repositories
{
    public class WalletLocationReceiptRepository : BaseRepository<WalletLocationReceipt, Guid>, IWalletLocationReceiptRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<WalletLocationReceipt> _dbSet;

        public WalletLocationReceiptRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.WalletLocationReceipts;
        }
    }
}

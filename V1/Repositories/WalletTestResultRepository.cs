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
    public class WalletTestResultRepository : BaseRepository<WalletTestResult, Guid>, IWalletTestResultRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<WalletTestResult> _dbset;

        public WalletTestResultRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbset = _context.WalletTestResults;
        }

        public async Task<List<WalletTestResult>> GetTestResults(Guid walletId)
        {
            var tests = await _dbset.Where(r => string.Equals(r.Wallet.Id.ToString(), walletId.ToString()))?.ToListAsync();
            return tests;
        }
    }
}

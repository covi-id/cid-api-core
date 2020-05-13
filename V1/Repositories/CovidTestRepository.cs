using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoviIDApiCore.V1.Repositories
{
    public class CovidTestRepository : BaseRepository<WalletTestResult, Guid>, ICovidTestRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<WalletTestResult> _dbSet;
        public CovidTestRepository(ApplicationDbContext context) :base(context)
        {
            _context = context;
            _dbSet = context.WalletTestResults;
        }
    }
}

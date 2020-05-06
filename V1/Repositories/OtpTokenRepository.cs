﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CoviIDApiCore.Data;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;

namespace CoviIDApiCore.V1.Repositories
{
    public class OtpTokenRepository : BaseRepository<OtpToken, BigInteger>, IOtpTokenRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<OtpToken> _dbSet;

        public OtpTokenRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.OtpTokens;
        }

        public async Task<OtpToken> GetUnusedByMobileNumber(string mobileNumber)
        {
            return await _dbSet
                .Where(t => string.Equals(t.MobileNumber, mobileNumber, StringComparison.Ordinal))
                .Where(t => !t.isUsed)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<OtpToken> GetBySessionId(string sessionId)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.SessionId == sessionId);
        }

        public async Task<List<OtpToken>> GetAllUnexpiredByMobileNumberAsync(string mobileNumber)
        {
            return await _dbSet
                .Where(t => t.MobileNumber == mobileNumber)
                .Where(t => t.ExpireAt < DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

//        public async Task<OtpToken> GetUnusedByWalletIdAndMobileNumber(string walletId, string mobileNumber)
//        {
//            return await _dbSet
//                .Where(t => string.Equals(t.Wallet.WalletIdentifier, walletId, StringComparison.Ordinal))
//                .Where(t => string.Equals(t.MobileNumber, mobileNumber, StringComparison.Ordinal))
//                .Where(t => !t.isUsed)
//                .OrderByDescending(t => t.CreatedAt)
//                .FirstOrDefaultAsync();
//        }
    }
}

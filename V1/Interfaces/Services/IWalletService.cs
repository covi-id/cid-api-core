﻿using CoviIDApiCore.V1.DTOs.Credentials;
using CoviIDApiCore.V1.DTOs.Wallet;
using System;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IWalletService
    {
        Task<WalletStatusResponse> GetWalletStatus(Guid walletId, string key);
        Task<TokenResponse> CreateWallet(CreateWalletRequest walletRequest);
        Task<CoviIdWalletContract> CreateCoviIdWallet(CoviIdWalletParameters coviIdWalletParameters);
    }
}

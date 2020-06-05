﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IWalletDetailService
    {
        Task<WalletDetail> CreateWalletDetails(Wallet wallet, WalletDetailsRequest walletDetails);
        Task<WalletDetail> CreateMobileWalletDetails(Wallet wallet, string mobileNumber);
        Task<List<WalletDetail>> GetWalletDetailsByEncryptedMobileNumber(string encryptedMobileNumber);
        Task DeleteWalletDetails(Wallet wallet);
    }
}
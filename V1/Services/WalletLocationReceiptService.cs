using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Services
{
    public class WalletLocationReceiptService : IWalletLocationReceiptService
    {
        private readonly IWalletLocationReceiptRepository _walletLocationReceiptRepository;
        private readonly IStaySafeService _staySafeService;
        private readonly ITestResultService _testResultService;

        public WalletLocationReceiptService(IWalletLocationReceiptRepository walletLocationReceiptRepository, IStaySafeService staySafeService, ITestResultService testResultService)
        {
            _walletLocationReceiptRepository = walletLocationReceiptRepository;
            _staySafeService = staySafeService;
            _testResultService = testResultService;
        }

        public async Task<WalletLocationReceipt> CreateReceipt(Wallet wallet, decimal longitude, decimal latitude, ScanType scanType)
        {
            var receipt = new WalletLocationReceipt
            {
                Wallet = wallet,
                Latitude = latitude,
                Longitude = longitude,
                ScanType = scanType
            };

            await _walletLocationReceiptRepository.AddAsync(receipt);
            
            await _walletLocationReceiptRepository.SaveAsync();

            BackgroundJob.Enqueue(() => CaptureDataIfPermitted(wallet.Id, scanType));

            return receipt;
        }

        public async Task<List<WalletLocationReceipt>> GetReceiptsForDate(Wallet wallet, DateTime forDate)
        {
            var receipts = await _walletLocationReceiptRepository.GetReceiptsForDate(wallet, forDate);

            return receipts;
        }

        #region Private Methods
        public async Task CaptureDataIfPermitted(Guid walletId, ScanType scanType)
        {
            if (scanType == ScanType.CheckIn)
            {
                var testResult = await _testResultService.GetLatestTestResult(walletId);

                if (testResult.HasConsent)
                {
                    await _staySafeService.CaptureData(walletId, DateTime.UtcNow);
                }
            }
        }
        #endregion
    }
}

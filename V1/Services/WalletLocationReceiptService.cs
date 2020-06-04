using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Services
{
    public class WalletLocationReceiptService : IWalletLocationReceiptService
    {
        private readonly IWalletLocationReceiptRepository _walletLocationReceiptRepository;

        public WalletLocationReceiptService(IWalletLocationReceiptRepository walletLocationReceiptRepository)
        {
            _walletLocationReceiptRepository = walletLocationReceiptRepository;
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
            
            return receipt;
        }

        public async Task<List<WalletLocationReceipt>> GetReceiptsForToday(Wallet wallet)
        {
            var receipts = await _walletLocationReceiptRepository.GetReceiptsForTodayByWallet(wallet);

            return receipts;
        }
    }
}

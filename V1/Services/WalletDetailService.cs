using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;

namespace CoviIDApiCore.V1.Services
{
    public class WalletDetailService : IWalletDetailService
    {
        private readonly IWalletDetailRepository _walletDetailRepository;

        public WalletDetailService(IWalletDetailRepository walletDetailRepository)
        {
            _walletDetailRepository = walletDetailRepository;
        }

        public async Task AddWalletDetailsAsync(Wallet wallet, WalletDetailsRequest walletDetails)
        {
            //TODO: Validation

            var details = new WalletDetail(walletDetails)
            {
                Wallet = wallet
            };

            await _walletDetailRepository.AddAsync(details);

            await _walletDetailRepository.SaveAsync();
        }
    }
}
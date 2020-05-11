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
        private readonly ICryptoService _cryptoService;

        public WalletDetailService(IWalletDetailRepository walletDetailRepository, ICryptoService cryptoService)
        {
            _walletDetailRepository = walletDetailRepository;
            _cryptoService = cryptoService;
        }

        public async Task AddWalletDetailsAsync(Wallet wallet, WalletDetailsRequest walletDetails, string key)
        {
            //TODO: Validation

            var details = new WalletDetail(walletDetails)
            {
                Wallet = wallet
            };

            _cryptoService.EncryptAsUser(details, key);

            await _walletDetailRepository.AddAsync(details);

            await _walletDetailRepository.SaveAsync();
        }
    }
}
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
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

        public async Task<WalletDetail> CreateWalletDetails(Wallet wallet, WalletDetailsRequest request, string key)
        {
            var walletDetails = new WalletDetail(request)
            {
                Wallet = wallet
            };

            if (walletDetails == null || walletDetails == default)
                throw new ValidationException(Messages.WalltDetails_Invalid);

            _cryptoService.EncryptAsUser(walletDetails, key);

            await _walletDetailRepository.AddAsync(walletDetails);

            await _walletDetailRepository.SaveAsync();

            return walletDetails;
        }

        public async Task DeleteWalletDetails(Wallet wallet)
        {
            var walletDetails = await _walletDetailRepository.GetWalletDetailsByWallet(wallet);

            if (walletDetails == null || walletDetails.Count < 1)
                return;

            _walletDetailRepository.DeleteRange(walletDetails);
            await _walletDetailRepository.SaveAsync();
            return;
        }
    }
}
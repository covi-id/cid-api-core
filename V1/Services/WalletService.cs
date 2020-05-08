using System;
using CoviIDApiCore.V1.DTOs.Credentials;
using CoviIDApiCore.V1.DTOs.Wallet;
using CoviIDApiCore.V1.Interfaces.Brokers;
using CoviIDApiCore.V1.Interfaces.Services;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace CoviIDApiCore.V1.Services
{
    public class WalletService : IWalletService
    {
        private readonly ICustodianBroker _custodianBroker;
        private readonly IAgencyBroker _agencyBroker;
        private readonly IConnectionService _connectionService;
        private readonly ICredentialService _credentialService;
        private readonly IConfiguration _configuration;
        private readonly IOtpService _otpService;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletDetailRepository _walletDetailRepository;
        private readonly ITestResultService _testResultService;
        
        public WalletService(ICustodianBroker custodianBroker, IConnectionService connectionService, IAgencyBroker agencyBroker,
            IConfiguration configuration, IOtpService otpService, IWalletRepository walletRepository,
            ICredentialService credentialService, IWalletDetailRepository walletDetailRepository,
            ITestResultService testResultService)
        {
            _custodianBroker = custodianBroker;
            _connectionService = connectionService;
            _agencyBroker = agencyBroker;
            _configuration = configuration;
            _credentialService = credentialService;
            _walletDetailRepository = walletDetailRepository;
            _testResultService = testResultService;
            _otpService = otpService;
            _walletRepository = walletRepository;
        }

        public async Task<WalletStatusResponse> GetWalletStatus(Guid walletId, string key)
        {
            // TODO : Handle decryption
            // TODO : Make photo url secure

            var wallet = await _walletDetailRepository.GetAsync(walletId);
            var testResults = await _testResultService.GetTestResult(walletId);

            var response = new WalletStatusResponse
            {
                FirstName = wallet.FirstName,
                LastName = wallet.LastName,
                PhotoUrl = wallet.PhotoUrl,
                ResultStatus = testResults.ResultStatus.ToString(),
                Status = (int)testResults.ResultStatus
            };
            return response;
        }

        public async Task<WalletResponse> CreateWallet(CreateWalletRequest walletRequest)
        {
            var otpReturn = await _otpService.GenerateAndSendOtpAsync(walletRequest.MobileNumber);

            var wallet = new Wallet
            {
                CreatedAt = DateTime.UtcNow,
                MobileNumber = walletRequest.MobileNumber,
                MobileNumberReference = walletRequest.MobileNumberReference,
                SessionId = otpReturn.SessionId
            };

            await _walletRepository.AddAsync(wallet);

            await _walletRepository.SaveAsync();

            return new WalletResponse
            {
                Token = otpReturn.AuthToken
            };
        }

        public async Task<CoviIdWalletContract> CreateCoviIdWallet(CoviIdWalletParameters coviIdWalletParameters)
        {
            var wallet = new WalletParameters
            {
                OwnerName = $"{coviIdWalletParameters.FirstName?.Trim()}-{coviIdWalletParameters.LastName?.Trim()}"
            };

            var response = await _custodianBroker.CreateWallet(wallet);

            var pictureUrl = await _agencyBroker.UploadFiles(coviIdWalletParameters.Photo, response.WalletId);

            var newWallet = await SaveNewWalletAsync(response.WalletId);

            await _otpService.GenerateAndSendOtpAsync(newWallet.Id.ToString());
            
            var contract = new CoviIdWalletContract
            {
                CovidStatusUrl = $"{_configuration.GetValue<string>("CoviIDBaseUrl")}/api/verifier/{response.WalletId}/covid-credentials",
                Picture = pictureUrl,
                WalletId = response.WalletId
            };

            return contract;
        }

        #region Private Methods
        private async Task<Wallet> SaveNewWalletAsync(string walletId)
        {
            var newWallet = new Wallet()
            {
                //Id = walletId,
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.AddAsync(newWallet);

            await _walletRepository.SaveAsync();

            return newWallet;
        }
        #endregion
    }
}
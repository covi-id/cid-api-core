using System;
using System.Linq;
using CoviIDApiCore.V1.DTOs.Wallet;
using CoviIDApiCore.V1.Interfaces.Services;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.WalletTestResult;
using CoviIDApiCore.V1.Interfaces.Brokers;

namespace CoviIDApiCore.V1.Services
{
    public class WalletService : IWalletService
    {
        private readonly IOtpService _otpService;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletDetailRepository _walletDetailRepository;
        private readonly ITestResultService _testResultService;
        private readonly IWalletDetailService _walletDetailService;
        private readonly ITokenService _tokenService;
        private readonly ICryptoService _cryptoService;
        private readonly IAmazonS3Broker _amazonS3Broker;
        private readonly ISessionService _sessionService;
        private readonly ISmsService _smsService;

        public WalletService(IOtpService otpService, IWalletRepository walletRepository, IWalletDetailRepository walletDetailRepository,
            ITestResultService testResultService, ITokenService tokenService, ICryptoService cryptoService,
            IAmazonS3Broker amazonS3Broker, ISessionService sessionService, IWalletDetailService walletDetailService,
            ISmsService smsService)
        {
            _walletDetailRepository = walletDetailRepository;
            _testResultService = testResultService;
            _tokenService = tokenService;
            _cryptoService = cryptoService;
            _amazonS3Broker = amazonS3Broker;
            _sessionService = sessionService;
            _walletDetailService = walletDetailService;
            _smsService = smsService;
            _otpService = otpService;
            _walletRepository = walletRepository;
        }

        public async Task<WalletStatusResponse> GetWalletStatus(string walletId, string key)
        {
            var wallet = await _walletRepository.GetAsync(Guid.Parse(walletId));
            if (wallet == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            var walletDetails = (await _walletDetailRepository.GetWalletDetailsByWallet(wallet)).FirstOrDefault();

            if (walletDetails == null)
                throw new NotFoundException(Messages.WalltDetails_NotFound);

            _cryptoService.DecryptAsUser(walletDetails, key);

            var photoUrl = _amazonS3Broker.GetImage(walletDetails.PhotoReference);

            var testResults = await _testResultService.GetLatestTestResult(Guid.Parse(walletId));

            var response = new WalletStatusResponse
            {
                FirstName = walletDetails.FirstName,
                LastName = walletDetails.LastName,
                PhotoUrl = photoUrl,
                ResultStatus = testResults == null ? ResultStatus.Untested.ToString() : testResults.ResultStatus.ToString(),
                Status = testResults == null ? Convert.ToInt32(ResultStatus.Untested) : (int)testResults?.ResultStatus
            };
            return response;
        }

        public async Task<TokenResponse> CreateWalletAndOtp(CreateWalletRequest walletRequest, string sessionId = null)
        {
            Wallet wallet;

            if (sessionId == null)
                wallet = await CreateWallet();
            else
            {
                wallet = await GetWallet(sessionId);
            }

            var otpId = await _otpService.GenerateAndSendOtpAsync(walletRequest.MobileNumber);

            var token = _tokenService.GenerateToken(wallet.Id.ToString(), otpId);

            return new TokenResponse
            {
                Token = token
            };
        }

        public async Task<Wallet> CreateMobileWallet(CreateWalletRequest request, string organisationName)
        {
            var wallet = await CreateWallet();

            var session = await _sessionService.CreateSession(request.MobileNumber, wallet);

            await _smsService.SendWelcomeSms(request.MobileNumber, organisationName, session.ExpireAt.Value, session.Id);

            await _walletDetailService.CreateMobileWalletDetails(wallet, request.MobileNumber);

            return wallet;
        }

        public async Task<Wallet> GetWalletByMobileNumebr(string mobileNumber)
        {
            var walletDetails = await _walletDetailService.GetWalletDetailsByMobileNumber(mobileNumber);

            // TODO better identify the wallet to checkout
            var wallet = walletDetails
                .OrderByDescending(wd => wd.CreatedAt)
                .FirstOrDefault()?
                .Wallet;

            if (wallet == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            return wallet;
        }

        public async Task DeleteWallet(string walletId)
        {
            var wallet = await _walletRepository.GetAsync(Guid.Parse(walletId));

            if (wallet == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            await _walletDetailService.DeleteWalletDetails(wallet);

            await _testResultService.DeleteTestResults(wallet.Id);

            _walletRepository.Delete(wallet);

            await _walletRepository.SaveAsync();

            return;
        }

        #region Private Methods
        private async Task<Wallet> CreateWallet()
        {
            var wallet = new Wallet
            {
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.AddAsync(wallet);

            await _walletRepository.SaveAsync();

            return wallet;
        }


        private async Task<Wallet> GetWallet(string sessionId)
        {
            var session = await _sessionService.GetAndUseSession(sessionId);

            var wallet = await _walletRepository.GetAsync(session.Wallet.Id);

            if (wallet == default)
                throw new NotFoundException(Messages.Wallet_NotFound);

            return wallet;
        }
        #endregion
    }
}
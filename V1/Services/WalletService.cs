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
using System.Collections.Generic;
using System.Security.Claims;

namespace CoviIDApiCore.V1.Services
{
    public class WalletService : IWalletService
    {
        private readonly IOtpService _otpService;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletDetailRepository _walletDetailRepository;
        private readonly ITestResultService _testResultService;
        private readonly ITokenService _tokenService;
        private readonly ICryptoService _cryptoService;
        private readonly IAmazonS3Broker _amazonS3Broker;
        private readonly ISessionService _sessionService;

        public WalletService(IOtpService otpService, IWalletRepository walletRepository, IWalletDetailRepository walletDetailRepository,
            ITestResultService testResultService, ITokenService tokenService, ICryptoService cryptoService,
            IAmazonS3Broker amazonS3Broker, ISessionService sessionService)
        {
            _walletDetailRepository = walletDetailRepository;
            _testResultService = testResultService;
            _tokenService = tokenService;
            _cryptoService = cryptoService;
            _amazonS3Broker = amazonS3Broker;
            _sessionService = sessionService;
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

            var testResults = await _testResultService.GetTestResult(Guid.Parse(walletId));

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

        public async Task<TokenResponse> CreateWalletAndOtp(CreateWalletRequest walletRequest, string sessionId)
        {
            Wallet wallet;

            if (sessionId == null)
                wallet = await CreateWallet(walletRequest);
            else
            {
                wallet = await GetWallet(sessionId);

                _cryptoService.DecryptAsServer(wallet);

                wallet.MobileNumberReference = walletRequest.MobileNumberReference;
                wallet.MobileNumber = wallet.MobileNumber;

                await UpdateWallet(wallet);
            }

            var otpId = await _otpService.GenerateAndSendOtpAsync(walletRequest.MobileNumber);

            return new TokenResponse
            {
                Token = _tokenService.GenerateToken(wallet.Id.ToString(), otpId)
            };
        }

        public async Task<Wallet> CreateWallet(CreateWalletRequest walletRequest, bool isStatic = false)
        {
            var wallet = new Wallet
            {
                CreatedAt = DateTime.UtcNow,
                MobileNumber = walletRequest.MobileNumber,
                MobileNumberReference = walletRequest?.MobileNumberReference
            };

            _cryptoService.EncryptAsServer(wallet, isStatic);

            await _walletRepository.AddAsync(wallet);

            await _walletRepository.SaveAsync();

            return wallet;
        }

        public async Task<TokenResponse> DeleteWalletAndOtpRequest(DeleteWalletAndOtpRequest request)
        {
            var claims = new List<Claim>();

            _cryptoService.EncryptAsServer(request);

            var wallets = await _walletRepository.GetListByEncryptedMobileNumber(request.MobileNumber);

            if (wallets == default || wallets == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            var otpId = await _otpService.GenerateAndSendOtpAsync(request.MobileNumber);

            foreach (var wallet in wallets)
            {
                claims.Add(new Claim(ClaimTypes.Name, wallet.Id.ToString()));
            }

            claims.Add(new Claim(ClaimTypes.Sid, otpId.ToString()));
            
            var token = _tokenService.CreateToken(claims);

            return new TokenResponse
            {
                Token = token
            };
        }
        
        public async Task DeleteAllWalletData(List<Guid> walletIds)
        {
            foreach (var walletId in walletIds)
            {
                var walletDetail = await _walletDetailRepository.GetAsync(walletId);
                
                _walletDetailRepository.Delete(walletDetail);

                var wallet = await _walletRepository.GetAsync(walletId);
                
                wallet.MobileNumber = null;
                wallet.MobileNumberReference = null;

                _walletRepository.Update(wallet);

                // TODO : check for data in OTP table
            }
            return;
        }

        #region Private Methods
        private async Task<Wallet> GetWallet(string sessionId)
        {
            var session = await _sessionService.GetAndUseSession(sessionId);

            var wallet = await _walletRepository.GetAsync(session.Wallet.Id);

            if (wallet == default)
                throw new NotFoundException(Messages.Wallet_NotFound);

            return wallet;
        }

        private async Task UpdateWallet(Wallet wallet)
        {
            _cryptoService.EncryptAsServer(wallet);

            _walletRepository.Update(wallet);

            await _walletRepository.SaveAsync();
        }
        #endregion
    }
}
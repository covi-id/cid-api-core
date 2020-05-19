using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.Interfaces.Brokers;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using Hangfire;
using Microsoft.Extensions.Configuration;

namespace CoviIDApiCore.V1.Services
{
    public class OtpService : IOtpService
    {
        private readonly IConfiguration _configuration;
        private readonly IOtpTokenRepository _otpTokenRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITestResultService _testResultService;
        private readonly IWalletDetailService _walletDetailService;
        private readonly ICryptoService _cryptoService;
        private readonly IAmazonS3Broker _amazonS3Broker;
        private readonly ITokenService _tokenService;
        private readonly ISmsService _smsService;
        private readonly IWalletService _walletService;

        public OtpService(IOtpTokenRepository tokenRepository, IConfiguration configuration, IWalletRepository walletRepository, 
            ITestResultService testResultService, IWalletDetailService walletDetailService, ICryptoService cryptoService, 
            ITokenService tokenService, IAmazonS3Broker amazonS3Broker, ISmsService smsService,
            IWalletService walletService)
        {
            _otpTokenRepository = tokenRepository;
            _configuration = configuration;
            _walletRepository = walletRepository;
            _testResultService = testResultService;
            _walletDetailService = walletDetailService;
            _cryptoService = cryptoService;
            _tokenService = tokenService;
            _amazonS3Broker = amazonS3Broker;
            _smsService = smsService;
            _walletService = walletService;
        }

        public async Task<long> GenerateAndSendOtpAsync(string mobileNumber)
        {
            var sms = await _smsService.SendOtpSms(mobileNumber);

            return await SaveOtpAsync(mobileNumber, sms.Code, sms.ValidityPeriod);
        }

        public async Task<TokenResponse> ResendOtpAsync(RequestResendOtp payload, string authToken)
        {
            var authTokenDetails = _tokenService.GetDetailsFromToken(authToken)?.FirstOrDefault();

            if (!await ValidateOtpCreationAsync(payload.MobileNumber))
                throw new ValidationException(Messages.Token_OTPThreshold);

            var wallet = await _walletRepository.GetAsync(Guid.Parse(authTokenDetails.WalletId));

            if (wallet == default)
                throw new NotFoundException(Messages.Wallet_NotFound);

            var otp = await GenerateAndSendOtpAsync(payload.MobileNumber);

            return new TokenResponse()
            {
                Token = _tokenService.GenerateToken(wallet.Id.ToString(), otp)
            };
        }

        //TODO: Improve this
        public async Task<OtpConfirmationResponse> ConfirmOtpAsync(RequestOtpConfirmation payload, string authToken)
        {
            if (payload.TestResult != null && !payload.isValid())
                throw new ValidationException(Messages.Token_InvaldPayload);

            var authTokenDetails = _tokenService.GetDetailsFromToken(authToken)?.FirstOrDefault();

            var token = await _otpTokenRepository.GetAsync(authTokenDetails.OtpId);

            await ValidateAndUpdateToken(token, payload.Otp);

            var wallet = await _walletRepository.GetAsync(Guid.Parse(authTokenDetails.WalletId));

            if (wallet == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            wallet.MobileNumberVerifiedAt = DateTime.UtcNow;

            _walletRepository.Update(wallet);

            await _walletRepository.SaveAsync();

            var fileReference = await _amazonS3Broker.AddImageToBucket(payload.WalletDetails.Photo, Guid.NewGuid().ToString());
            payload.WalletDetails.Photo = fileReference;

            var key = _cryptoService.GenerateEncryptedSecretKey();

            await _walletDetailService.AddWalletDetailsAsync(wallet, payload.WalletDetails, key);

            if (payload.TestResult != null)
                await _testResultService.AddTestResult(wallet, payload.TestResult);

            return new OtpConfirmationResponse()
            {
                WalletId = wallet.Id.ToString(),
                Key = key
            };
        }

        public async Task ConfirmDeleteWallet(OtpDeleteWalletRequest request, string authToken)
        {
            var authTokenDetails = _tokenService.GetDetailsFromToken(authToken);
            
            var otpId = authTokenDetails.FirstOrDefault().OtpId;
            
            var otpToken = await _otpTokenRepository.GetAsync(otpId);

            await ValidateAndUpdateToken(otpToken, request.Code);
            var walletIds = new List<Guid>();

            foreach(var token in authTokenDetails)
            {
                walletIds.Add(Guid.Parse(token.WalletId));
            }

            BackgroundJob.Enqueue(() => _walletService.DeleteAllWalletData(walletIds));
            return;
        }

        #region Private Methods
        private async Task<long> SaveOtpAsync(string mobileNumber, int code, int expiryTime)
        {
            var newToken = new OtpToken()
            {
                Code = code,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddMinutes(expiryTime),
                isUsed = false,
                MobileNumber = mobileNumber
            };

            await _otpTokenRepository.AddAsync(newToken);

            await _otpTokenRepository.SaveAsync();

            return newToken.Id;
        }

        private async Task<bool> ValidateOtpCreationAsync(string mobileNumberReference)
        {
            var otps = await _otpTokenRepository.GetAllUnexpiredByMobileNumberAsync(mobileNumberReference);

            if (!otps.Any())
                return true;

            var timeThreshold = _configuration.GetValue<int>("OTPSettings:TimeThreshold");

            var amountThreshold = _configuration.GetValue<int>("OTPSettings:AmountThreshold");

            return otps.Count(otp => otp.CreatedAt > DateTime.UtcNow.AddMinutes(-1 * timeThreshold)) <= amountThreshold;
        }

        private async Task ValidateAndUpdateToken(OtpToken token, int code)
        {
            if (token == default || token.isUsed || token.ExpireAt <= DateTime.UtcNow)
                throw new ValidationException(Messages.Token_OTPNotExist);

            if (token.Code != code)
                throw new ValidationException(Messages.Token_OTPFailed);

            token.isUsed = true;

            _otpTokenRepository.Update(token);

            await _otpTokenRepository.SaveAsync();
        }

        #endregion
    }
}
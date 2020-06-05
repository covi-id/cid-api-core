using System;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.Interfaces.Brokers;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace CoviIDApiCore.V1.Services
{
    public class OtpService : IOtpService
    {
        private readonly IConfiguration _configuration;
        private readonly IOtpTokenRepository _otpTokenRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletDetailService _walletDetailService;
        private readonly ICryptoService _cryptoService;
        private readonly IAmazonS3Broker _amazonS3Broker;
        private readonly ITokenService _tokenService;
        private readonly ISmsService _smsService;

        public OtpService(IOtpTokenRepository tokenRepository, IConfiguration configuration, IWalletRepository walletRepository, 
            IWalletDetailService walletDetailService, ICryptoService cryptoService, ITokenService tokenService, IAmazonS3Broker amazonS3Broker, 
            ISmsService smsService)
        {
            _otpTokenRepository = tokenRepository;
            _configuration = configuration;
            _walletRepository = walletRepository;
            _walletDetailService = walletDetailService;
            _cryptoService = cryptoService;
            _tokenService = tokenService;
            _amazonS3Broker = amazonS3Broker;
            _smsService = smsService;
        }

        public async Task<long> GenerateAndSendOtpAsync(string mobileNumber)
        {
            var sms = await _smsService.SendOtpSms(mobileNumber);

            return await SaveOtpAsync(mobileNumber, sms.Code, sms.ValidityPeriod);
        }

        public async Task<TokenResponse> ResendOtpAsync(RequestResendOtp payload, string authToken)
        {
            var authTokenDetails = _tokenService.GetDetailsFromToken(authToken);

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

        public async Task<OtpConfirmationResponse> ConfirmOtpAsync(RequestOtpConfirmation payload, string authToken)
        {
            var authTokenDetails = _tokenService.GetDetailsFromToken(authToken);

            var token = await _otpTokenRepository.GetAsync(authTokenDetails.OtpId);

            if (token == default || token.isUsed || token.ExpireAt <= DateTime.UtcNow || token.Code != payload.Otp)
                throw new ValidationException(Messages.Token_OTPNotExist);

            token.isUsed = true;

            _otpTokenRepository.Update(token);

            await _otpTokenRepository.SaveAsync();

            var wallet = await UpdateWalletToVerified(authTokenDetails.WalletId);

            var fileReference = await _amazonS3Broker.AddImageToBucket(payload.WalletDetails.Photo, Guid.NewGuid().ToString());
            payload.WalletDetails.Photo = fileReference;

            await _walletDetailService.CreateWalletDetails(wallet, payload.WalletDetails);

            return new OtpConfirmationResponse()
            {
                WalletId = wallet.Id.ToString(),
            };
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

            _cryptoService.EncryptAsServer(newToken);

            await _otpTokenRepository.AddAsync(newToken);

            await _otpTokenRepository.SaveAsync();

            return newToken.Id;
        }

        private async Task<bool> ValidateOtpCreationAsync(string mobileNumberReference)
        {
            _cryptoService.EncryptAsServer(mobileNumberReference);

            var otps = await _otpTokenRepository.GetAllUnexpiredByEncryptedMobileNumber(mobileNumberReference);

            if (!otps.Any())
                return true;

            var timeThreshold = _configuration.GetValue<int>("OTPSettings:TimeThreshold");

            var amountThreshold = _configuration.GetValue<int>("OTPSettings:AmountThreshold");

            return otps.Count(otp => otp.CreatedAt > DateTime.UtcNow.AddMinutes(-1 * timeThreshold)) <= amountThreshold;
        }

        private async Task<Wallet> UpdateWalletToVerified(string walletId)
        {
            var wallet = await _walletRepository.GetAsync(Guid.Parse(walletId));

            if (wallet == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            wallet.MobileNumberVerifiedAt = DateTime.UtcNow;

            _walletRepository.Update(wallet);

            await _walletRepository.SaveAsync();

            return wallet;
        }

        #endregion
    }
}
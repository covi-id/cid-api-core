using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.DTOs.Clickatell;
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
        private readonly IClickatellBroker _clickatellBroker;
        private readonly IWalletRepository _walletRepository;
        private readonly ITestResultService _testResultService;
        private readonly IWalletDetailService _walletDetailService;
        private readonly ICryptoService _cryptoService;
        private readonly ITokenService _tokenService;

        public OtpService(IOtpTokenRepository tokenRepository, IConfiguration configuration, IClickatellBroker clickatellBroker,
            IWalletRepository walletRepository, ITestResultService testResultService, IWalletDetailService walletDetailService, ICryptoService cryptoService, ITokenService tokenService)
        {
            _otpTokenRepository = tokenRepository;
            _configuration = configuration;
            _clickatellBroker = clickatellBroker;
            _walletRepository = walletRepository;
            _testResultService = testResultService;
            _walletDetailService = walletDetailService;
            _cryptoService = cryptoService;
            _tokenService = tokenService;
        }

        public async Task<long> GenerateAndSendOtpAsync(string mobileNumber)
        {
            var expiryTime = _configuration.GetValue<int>("OTPSettings:ValidityPeriod");

            var code = Utilities.Helpers.GenerateRandom4DigitNumber();

            var message = ConstructMessage(mobileNumber, code, expiryTime);

            await _clickatellBroker.SendSms(message);

            return await SaveOtpAsync(mobileNumber,code, expiryTime);
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

        public async Task<TokenResponse> ResendOtpAsync(RequestResendOtp payload, string authToken)
        {
            var authTokenDetails = _tokenService.GetDetailsFromToken(authToken);

            if(!await ValidateOtpCreationAsync(payload.MobileNumber))
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

        private ClickatellTemplate ConstructMessage(string mobileNumber, int code, int validityPeriod)
        {
            var recipient = new[]
            {
                mobileNumber
            };

            return new ClickatellTemplate()
            {
                To = recipient,
                Content = string.Format(_configuration.GetValue<string>("OTPSettings:Message"), code.ToString(), validityPeriod.ToString())
            };
        }

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

        //TODO: Improve this
        public async Task<OtpConfirmationResponse> ConfirmOtpAsync(RequestOtpConfirmation payload, string authToken)
        {
            if (!payload.isValid())
                throw new ValidationException(Messages.Token_InvaldPayload);

            var authTokenDetails = _tokenService.GetDetailsFromToken(authToken);

            var token = await _otpTokenRepository.GetAsync(authTokenDetails.OtpId);

            if (token == default || token.ExpireAt <= DateTime.UtcNow || token.Code != payload.Otp)
                throw new ValidationException(Messages.Token_OTPNotExist);

            token.isUsed = true;

            _otpTokenRepository.Update(token);

            await _otpTokenRepository.SaveAsync();

            var wallet = await _walletRepository.GetAsync(Guid.Parse(authTokenDetails.WalletId));

            if (wallet == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            wallet.MobileNumberVerifiedAt = DateTime.UtcNow;

            _walletRepository.Update(wallet);

            await _walletRepository.SaveAsync();

            //TODO: Upload photo

            payload.WalletDetails.Photo = "New photo URL";

            var key = _cryptoService.GenerateEncryptedSecretKey();

            await _walletDetailService.AddWalletDetailsAsync(wallet, payload.WalletDetails, key);

            if(payload.TestResult != null)
                await _testResultService.AddTestResult(wallet, payload.TestResult);

            return new OtpConfirmationResponse()
            {
                WalletId = wallet.Id.ToString(),
                Key = key
            };
        }
    }
}
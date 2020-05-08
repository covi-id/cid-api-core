using System;
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
        private readonly IAmazonS3Broker _amazonS3Broker;

        public OtpService(IOtpTokenRepository tokenRepository, IConfiguration configuration, IClickatellBroker clickatellBroker,
            IWalletRepository walletRepository, ITestResultService testResultService, 
            IWalletDetailService walletDetailService, ICryptoService cryptoService, IAmazonS3Broker amazonS3Broker)
        {
            _otpTokenRepository = tokenRepository;
            _configuration = configuration;
            _clickatellBroker = clickatellBroker;
            _walletRepository = walletRepository;
            _testResultService = testResultService;
            _walletDetailService = walletDetailService;
            _cryptoService = cryptoService;
            _amazonS3Broker = amazonS3Broker;
        }

        public async Task<string> GenerateAndSendOtpAsync(string mobileNumber)
        {
            var expiryTime = _configuration.GetValue<int>("OTPSettings:ValidityPeriod");

            var code = Utilities.Helpers.GenerateRandom4DigitNumber();

            var sessionId = Utilities.Helpers.GenerateSessionToken();

            var message = ConstructMessage(mobileNumber, code, expiryTime);

            await _clickatellBroker.SendSms(message);

            await SaveOtpAsync(mobileNumber, sessionId, code, expiryTime);

            return sessionId;
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

        public async Task ResendOtp(RequestResendOtp payload)
        {
            if(!await ValidateOtpCreationAsync(payload.MobileNumber))
                throw new ValidationException(Messages.Token_OTPThreshold);

            var wallet = await _walletRepository.GetBySessionId(payload.SessionId);

            if (wallet == default)
                throw new NotFoundException(Messages.Wallet_NotFound);

            var sessionId = await GenerateAndSendOtpAsync(payload.MobileNumber);

            wallet.SessionId = sessionId;

            _walletRepository.Update(wallet);

            await _walletRepository.SaveAsync();
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

        private async Task SaveOtpAsync(string mobileNumber, string sessionId, int code, int expiryTime)
        {
            var newToken = new OtpToken()
            {
                Code = code,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddMinutes(expiryTime),
                isUsed = false,
                MobileNumber = mobileNumber,
                SessionId = sessionId
            };

            await _otpTokenRepository.AddAsync(newToken);

            await _otpTokenRepository.SaveAsync();
        }

        //TODO: Improve this
        public async Task<OtpConfirmationResponse> ConfirmOtpAsync(RequestOtpConfirmation payload)
        {
            if (!payload.isValid())
                throw new ValidationException(Messages.Token_InvaldPayload);

            var token = await _otpTokenRepository.GetBySessionId(payload.SessionId);

            if (token == default || token.ExpireAt <= DateTime.UtcNow || token.Code != payload.Otp)
                throw new ValidationException(Messages.Token_OTPNotExist);

            token.isUsed = true;

            _otpTokenRepository.Update(token);

            await _otpTokenRepository.SaveAsync();

            var wallet = await _walletRepository.GetBySessionId(payload.SessionId);

            if (wallet == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            wallet.MobileNumberVerifiedAt = DateTime.UtcNow;

            _walletRepository.Update(wallet);

            await _walletRepository.SaveAsync();

            var fileReference = await _amazonS3Broker.AddImageToBucket(payload.WalletDetails.Photo, wallet.Id.ToString());
            payload.WalletDetails.Photo = fileReference;

            await _walletDetailService.AddWalletDetailsAsync(wallet, payload.WalletDetails);

            if(payload.TestResult != null)
                await _testResultService.AddTestResult(wallet, payload.TestResult);

            return new OtpConfirmationResponse()
            {
                WalletId = wallet.Id.ToString(),
                Key = _cryptoService.GenerateEncryptedSecretKey()
            };
        }
    }
}
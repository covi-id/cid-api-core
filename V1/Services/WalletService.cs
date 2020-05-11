﻿using System;
using CoviIDApiCore.V1.DTOs.Wallet;
using CoviIDApiCore.V1.Interfaces.Services;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.Interfaces.Brokers;

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

        public WalletService(IOtpService otpService, IWalletRepository walletRepository, IWalletDetailRepository walletDetailRepository,
            ITestResultService testResultService, ITokenService tokenService, ICryptoService cryptoService,
            IAmazonS3Broker amazonS3Broker)
        {
            _walletDetailRepository = walletDetailRepository;
            _testResultService = testResultService;
            _tokenService = tokenService;
            _cryptoService = cryptoService;
            _amazonS3Broker = amazonS3Broker;
            _otpService = otpService;
            _walletRepository = walletRepository;
        }

        public async Task<WalletStatusResponse> GetWalletStatus(Guid walletId, string key)
        {
            var wallet = await _walletDetailRepository.GetAsync(walletId);
            if (wallet == null)
                throw new ValidationException(Messages.Wallet_NotFound);

            _cryptoService.DecryptAsUser(wallet, key);

            var photoUrl = await _amazonS3Broker.GetImage(wallet.PhotoReference);

            var testResults = await _testResultService.GetTestResult(walletId);

            var response = new WalletStatusResponse
            {
                FirstName = wallet.FirstName,
                LastName = wallet.LastName,
                PhotoUrl = photoUrl,
                ResultStatus = testResults.ResultStatus.ToString(),
                Status = (int)testResults.ResultStatus
            };
            return response;
        }

        public async Task<TokenResponse> CreateWallet(CreateWalletRequest walletRequest)
        {
            var otpReturn = await _otpService.GenerateAndSendOtpAsync(walletRequest.MobileNumber);

            var wallet = new Wallet
            {
                CreatedAt = DateTime.UtcNow,
                MobileNumber = walletRequest.MobileNumber,
                MobileNumberReference = walletRequest.MobileNumberReference
            };

            _cryptoService.EncryptAsServer(wallet);

            await _walletRepository.AddAsync(wallet);

            await _walletRepository.SaveAsync();

            return new TokenResponse
            {
                Token = _tokenService.GenerateToken(wallet.Id.ToString(), otpReturn)
            };
        }
    }
}
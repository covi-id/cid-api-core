﻿using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.WalletTestResult;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using Microsoft.EntityFrameworkCore.Internal;
using Hangfire;

namespace CoviIDApiCore.V1.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly IWalletTestResultRepository _walletTestResultRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IStaySafeService _staySafeService;

        public TestResultService(IWalletTestResultRepository walletTestResultRepository, IWalletRepository walletRepository,
            IStaySafeService staySafeService)
        {
            _walletTestResultRepository = walletTestResultRepository;
            _walletRepository = walletRepository;
            _staySafeService = staySafeService;
        }

        public async Task<WalletTestResult> GetLatestTestResult(Guid walletId)
        {
            var tests = await _walletTestResultRepository.GetTestResults(walletId);

            if (tests == null)
                throw new NotFoundException(Messages.TestResult_NotFound);

            if (!tests.Any())
                return null;

            var test = tests.OrderByDescending(t => t.CreatedAt)?.FirstOrDefault();
            
            return test;
        }

        public async Task<WalletTestResult> AddTestResult(TestResultRequest request)
        {
            if (request == null || !request.isValid())
                throw new ValidationException(Messages.TestResult_Invalid);

            var wallet = await _walletRepository.GetAsync(request.walletId);

            if (wallet == null)
                throw new ValidationException(Messages.Wallet_NotFound);

            var testResults = new WalletTestResult(request, wallet);

            await _walletTestResultRepository.AddAsync(testResults);

            await _walletTestResultRepository.SaveAsync();

            if (request.ResultStatus == ResultStatus.Positive && request.HasConsent)
                BackgroundJob.Enqueue(() => _staySafeService.CaptureData(wallet.Id, request.TestedAt, -14));

            return testResults;
        }

        public async Task<bool> DeleteTestResults(Guid walletId)
        {
            var tests = await _walletTestResultRepository.GetTestResults(walletId);

            if (tests == null || tests.Count < 1)
                return false;

            _walletTestResultRepository.DeleteRange(tests);
            
            await _walletTestResultRepository.SaveAsync();
            
            return true;
        }
    }
}

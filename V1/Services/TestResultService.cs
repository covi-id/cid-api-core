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

namespace CoviIDApiCore.V1.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly IWalletTestResultRepository _walletTestResultRepository;
        private readonly IWalletRepository _walletRepository;
        public TestResultService(IWalletTestResultRepository walletTestResultRepository, IWalletRepository walletRepository)
        {
            _walletTestResultRepository = walletTestResultRepository;
            _walletRepository = walletRepository;
        }

        public async Task<TestResultResponse> GetTestResult(Guid walletId)
        {
            var tests = await _walletTestResultRepository.GetTestResults(walletId);

            if (tests == null)
                throw new ValidationException(Messages.TestResult_NotFound);

            if (!tests.Any())
                return null;

            var response = new TestResultResponse();

            if (tests.Count > 1)
            {
                // TODO : Do calculation based on all test results
            }
            var test = tests.OrderByDescending(t => t.IssuedAt).FirstOrDefault();
            response.HasConsent = test.HasConsent;
            response.IssuedAt = test.IssuedAt;
            response.Laboratory = test.Laboratory;
            response.LaboratoryStatus = test.LaboratoryStatus;
            response.PermissionGrantedAt = test.PermissionGrantedAt;
            response.ReferenceNumber = test.ReferenceNumber;
            response.ResultStatus = test.ResultStatus;
            response.TestedAt = test.TestedAt;

            return response;
        }

        public async Task AddTestResult(TestResultRequest testResultRequest)
        {
            if (testResultRequest == null || !testResultRequest.isValid())
                throw new ValidationException(Messages.Token_InvaldPayload);

            var wallet = await _walletRepository.GetAsync(testResultRequest.walletId);

            if (wallet == null)
                throw new ValidationException(Messages.Wallet_NotFound);

            var testResults = new WalletTestResult
            {
                Wallet = wallet,
                Laboratory = testResultRequest.Laboratory,
                ReferenceNumber = testResultRequest.ReferenceNumber,
                TestedAt = testResultRequest.TestedAt,
                ResultStatus = testResultRequest.ResultStatus,
                LaboratoryStatus = LaboratoryStatus.Unsent,
                TestType = TestType.Covid19,
                HasConsent = testResultRequest.HasConsent,
                PermissionGrantedAt = DateTime.UtcNow
            };

            await _walletTestResultRepository.AddAsync(testResults);

            await _walletTestResultRepository.SaveAsync();
        }

        public async Task DeleteTestResults(Guid walletId)
        {
            var tests = await _walletTestResultRepository.GetTestResults(walletId);

            if (tests == null || tests.Count < 1)
                return;

            _walletTestResultRepository.DeleteRange(tests);
            await _walletTestResultRepository.SaveAsync();
            return;
        }
    }
}

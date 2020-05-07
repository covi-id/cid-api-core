﻿using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.WalletTestResult;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly IWalletTestResultRepository _walletTestResultRepository;
        public TestResultService(IWalletTestResultRepository walletTestResultRepository)
        {
            _walletTestResultRepository = walletTestResultRepository;
        }

        public async Task<TestResultResponse> GetTestResult(Guid walletId)
        {
            var tests = await _walletTestResultRepository.GetTestResults(walletId);
            var response = new TestResultResponse();

            if (tests.Count > 1)
            {
                // TODO : Do calculation based on all test results

            }
            var test = tests.OrderBy(t => t.TestedAt).FirstOrDefault();
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
            var testResults = new WalletTestResult
            {
                //TODO: Get wallet via the secret key
                Laboratory = testResultRequest.Laboratory,
                ReferenceNumber = testResultRequest.ReferenceNumber,
                TestedAt = testResultRequest.TestedAt,
                IssuedAt = testResultRequest.IssuedAt,
                ResultStatus = testResultRequest.ResultStatus,
                LaboratoryStatus = LaboratoryStatus.Unsent,
                TestType = TestType.Covid19,
                HasConsent = testResultRequest.HasConsent,
                PermissionGrantedAt = DateTime.UtcNow
            };

            await _walletTestResultRepository.AddAsync(testResults);

            await _walletTestResultRepository.SaveAsync();
        }

        public async Task AddTestResult(Wallet wallet, TestResultRequest testResultRequest)
        {
            var testResults = new WalletTestResult
            {
                Wallet = wallet,
                Laboratory = testResultRequest.Laboratory,
                ReferenceNumber = testResultRequest.ReferenceNumber,
                TestedAt = testResultRequest.TestedAt,
                IssuedAt = testResultRequest.IssuedAt,
                ResultStatus = testResultRequest.ResultStatus,
                LaboratoryStatus = LaboratoryStatus.Unsent,
                TestType = TestType.Covid19,
                HasConsent = testResultRequest.HasConsent,
                PermissionGrantedAt = DateTime.UtcNow
            };

            await _walletTestResultRepository.AddAsync(testResults);

            await _walletTestResultRepository.SaveAsync();
        }
    }
}

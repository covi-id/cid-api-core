using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.WalletTestResult;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using Microsoft.EntityFrameworkCore.Internal;
using Amazon.Runtime;

namespace CoviIDApiCore.V1.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly IWalletTestResultRepository _walletTestResultRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICryptoService _cryptoService;
        public TestResultService(IWalletTestResultRepository walletTestResultRepository, IWalletRepository walletRepository, ICryptoService cryptoService)
        {
            _walletTestResultRepository = walletTestResultRepository;
            _walletRepository = walletRepository;
            _cryptoService = cryptoService;
        }

        public async Task<TestResultResponse> GetTestResult(Guid walletId)
        {
            var tests = await _walletTestResultRepository.GetTestResults(walletId);

            if (tests == null)
                throw new ValidationException(Messages.TestResult_NotFound);

            if (!tests.Any())
                return null;

            var response = new TestResultResponse();

            var test = tests.OrderByDescending(t => t.IssuedAt)?.FirstOrDefault();
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

        public async Task<WalletTestResult> AddTestResult(TestResultRequest request)
        {
            if (testResultRequest == null || !testResultRequest.isValid())
                throw new ValidationException(Messages.TestResult_Invalid);

            var wallet = await _walletRepository.GetAsync(request.walletId);

            if (wallet == null)
                throw new ValidationException(Messages.Wallet_NotFound);

            var testResults = new WalletTestResult(request, wallet);

            _cryptoService.EncryptAsUser(request, request.Key);

            await _walletTestResultRepository.AddAsync(testResults);

            await _walletTestResultRepository.SaveAsync();

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

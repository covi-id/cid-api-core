using CoviIDApiCore.V1.DTOs.WalletTestResult;
using System;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ITestResultService
    {
        Task<TestResultResponse> GetTestResult(Guid walletId);
        Task AddTestResult(TestResultRequest testResultRequest);
        Task AddTestResult(Wallet wallet, TestResultRequest testResultRequest);
        Task DeleteTestResults(Guid walletId);
    }
}

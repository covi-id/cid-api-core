using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.WalletTestResult;
using System;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ITestResultService
    {
        Task<TestResultResponse> GetTestResult(Guid walletId);
        Task<WalletTestResult> AddTestResult(TestResultRequest testResultRequest);
        Task<bool> DeleteTestResults(Guid walletId);
    }
}

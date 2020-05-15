using System;
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.SMS;
using CoviIDApiCore.V1.DTOs.System;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ISmsService
    {
        Task<SmsResponse> SendOtpSms(string mobileNumber);
        Task SendWelcomeSms(string mobileNumber, string organisationName, DateTime expireAt, Guid sessionId);
        Response CreateBalanceJob();
        Task SendBalanceSms();
        Task<Response> VerifyBalance();
        Response DeleteBalanceJob();
    }
}
using System;
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.SMS;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ISmsService
    {
        Task<SmsResponse> SendOtpSms(string mobileNumber);
        Task SendWelcomeSms(string mobileNumber, string organisationName, DateTime expireAt, Guid sessionId);
    }
}
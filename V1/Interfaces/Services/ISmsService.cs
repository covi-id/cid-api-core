using System;
using System.Threading.Tasks;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.SMS;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ISmsService
    {
        Task<SmsResponse> SendMessage(string mobileNumber, DefinitionConstants.SmsType smsType, string organisation = null, DateTime expireAt = default);
    }
}
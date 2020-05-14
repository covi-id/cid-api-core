using System.Threading.Tasks;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.SMS;
using CoviIDApiCore.V1.DTOs.System;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ISmsService
    {
        Task<SmsResponse> SendMessage(string mobileNumber, DefinitionConstants.SmsType smsType, string organisation = null, string url = null);
        Response CreateBalanceJob();
    }
}
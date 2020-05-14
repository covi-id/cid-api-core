using System;
using System.Threading.Tasks;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Clickatell;
using CoviIDApiCore.V1.DTOs.SMS;
using CoviIDApiCore.V1.Interfaces.Brokers;
using Microsoft.Extensions.Configuration;

using CoviIDApiCore.V1.Interfaces.Services;
using SmsType = CoviIDApiCore.V1.Constants.DefinitionConstants.SmsType;

namespace CoviIDApiCore.V1.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly IClickatellBroker _clickatellBroker;

        public SmsService(IConfiguration configuration, IClickatellBroker clickatellBroker)
        {
            _configuration = configuration;
            _clickatellBroker = clickatellBroker;
        }

        // TODO : Split up into 2 methods
        public async Task<SmsResponse> SendMessage(string mobileNumber, SmsType smsType, string organisation = null, DateTime expireAt = default, Guid sessionId = default)
        {
            ClickatellTemplate message;
            var validityPeriod = 0;
            var code = 0;

            switch (smsType)
            {
                case SmsType.Otp:
                    validityPeriod = _configuration.GetValue<int>("OTPSettings:ValidityPeriod");

                    code = Utilities.Helpers.GenerateRandom4DigitNumber();

                    message = ConstructOtpMessage(mobileNumber, code, validityPeriod);

                    break;
                case SmsType.Welcome:
                    // TODO : Shorten url with bitly
                    var url = _configuration.GetValue<string>("WebsiteDomian");
                    url = $"{url}/?sessionId={sessionId}";

                    message = ConstructWelcomeMessage(mobileNumber, organisation, url, expireAt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(smsType), smsType, null);
            }

            await _clickatellBroker.SendSms(message);

            return new SmsResponse()
            {
                Code = code,
                ValidityPeriod = validityPeriod
            };
        }

        private ClickatellTemplate ConstructWelcomeMessage(string mobileNumber, string organisation, string url, DateTime expireAt)
        {
            var recipient = new[]
            {
                mobileNumber
            };

            return new ClickatellTemplate()
            {
                To = recipient,
                Content = string.Format(DefinitionConstants.SmsStrings[SmsType.Welcome], organisation, url, expireAt)
            };
        }

        private ClickatellTemplate ConstructOtpMessage(string mobileNumber, int code, int validityPeriod)
        {
            var recipient = new[]
            {
                mobileNumber
            };

            return new ClickatellTemplate()
            {
                To = recipient,
                Content = string.Format(DefinitionConstants.SmsStrings[SmsType.Otp], code.ToString(), validityPeriod.ToString())
            };
        }
    }
}
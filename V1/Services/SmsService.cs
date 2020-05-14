using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Bitly;
using CoviIDApiCore.V1.DTOs.Clickatell;
using CoviIDApiCore.V1.DTOs.SMS;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.Interfaces.Brokers;
using Microsoft.Extensions.Configuration;

using CoviIDApiCore.V1.Interfaces.Services;
using Hangfire;
using Newtonsoft.Json;
using SmsType = CoviIDApiCore.V1.Constants.DefinitionConstants.SmsType;

namespace CoviIDApiCore.V1.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly IClickatellBroker _clickatellBroker;
        private readonly IBitlyBroker _bitlyBroker;

        public SmsService(IConfiguration configuration, IClickatellBroker clickatellBroker, IBitlyBroker bitlyBroker)
        {
            _configuration = configuration;
            _clickatellBroker = clickatellBroker;
            _bitlyBroker = bitlyBroker;
        }

        public async Task<SmsResponse> SendMessage(string mobileNumber, SmsType smsType, string organisation = null,
            string url = null)
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
                    if (organisation == default)
                        throw new ValidationException(Messages.Val_Organisation);

                    message = ConstructWelcomeMessage(mobileNumber, organisation, await GetShortenedUrl(url));
                    break;
                case SmsType.UpdateBalance:
                    message = ConstructUpdateBalanceMessage(mobileNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(smsType), smsType, null);
            }

            var balance = (CheckBalance()).Data as ClickatellResponse;

            if(balance.Balance < 1)
                    throw new ClickatellException(Messages.Clickatell_Balance);

            await _clickatellBroker.SendSms(message);

            return new SmsResponse()
            {
                Code = code,
                ValidityPeriod = validityPeriod
            };
        }

        private async Task<string> GetShortenedUrl<T>(T url)
        {
            if (EqualityComparer<T>.Default.Equals(url, default))
                throw new ValidationException(Messages.Val_Url);

            var payload = new BitlyTemplate()
            {
                Url = url.ToString()
            };

            var bitlyResponse = await _bitlyBroker.ShortenRequest(payload);

            return bitlyResponse.Link;
        }

        private static ClickatellTemplate ConstructWelcomeMessage(string mobileNumber, string organisation, string url)
        {
            var recipient = new[]
            {
                mobileNumber
            };

            return new ClickatellTemplate()
            {
                To = recipient,
                Content = string.Format(DefinitionConstants.SmsStrings[SmsType.Welcome], organisation, url)
            };
        }

        private static ClickatellTemplate ConstructOtpMessage(string mobileNumber, int code, int validityPeriod)
        {
            var recipient = new[]
            {
                mobileNumber
            };

            return new ClickatellTemplate()
            {
                To = recipient,
                Content = string.Format(DefinitionConstants.SmsStrings[SmsType.Otp], code.ToString(),
                    validityPeriod.ToString())
            };
        }

        private static ClickatellTemplate ConstructUpdateBalanceMessage(string mobileNumber)
        {
            var recipient = new[]
            {
                mobileNumber
            };

            return new ClickatellTemplate()
            {
                To = recipient,
                Content = DefinitionConstants.SmsStrings[SmsType.Welcome]
            };
        }

        public Response CheckBalance()
        {

        }

        public Response CreateBalanceJob()
        {
            RecurringJob.AddOrUpdate(() =>
                SendMessage(_configuration.GetValue<string>("PlatformSettings:BalanceNotificationNumber"), SmsType.UpdateBalance, null, null),
                Cron.Daily);

            return new Response(true, HttpStatusCode.Created);
        }
    }
}
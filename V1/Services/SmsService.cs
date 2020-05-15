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
using Hangfire.Storage;
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

        public async Task<SmsResponse> SendOtpSms(string mobileNumber)
        {
            var validityPeriod = _configuration.GetValue<int>("OTPSettings:ValidityPeriod");

            var code = Utilities.Helpers.GenerateRandom4DigitNumber();

            var message = ConstructOtpMessage(mobileNumber, code, validityPeriod);

            await VerifyBalance();

            await _clickatellBroker.SendSms(message);

            return new SmsResponse()
            {
                Code = code,
                ValidityPeriod = validityPeriod
            };
        }

        public async Task SendWelcomeSms(string mobileNumber, string organisationName, DateTime expireAt, Guid sessionId)
        {
            var url = $"{_configuration.GetValue<string>("WebsiteDomain")}{UrlConstants.PartialRoutes[UrlConstants.Routes.WebCreateWallet]}?sessionId={sessionId}";

            var message = ConstructWelcomeMessage(mobileNumber, organisationName, await GetShortenedUrl(url), expireAt);

            await VerifyBalance();

            await _clickatellBroker.SendSms(message);
        }

        public async Task SendBalanceSms()
        {
            var mobileNumber = _configuration.GetValue<string>("PlatformSettings:BalanceNotificationNumber");
            var threshold = _configuration.GetValue<int>("PlatformSettings:BalanceMinThreshold");

            var message = ConstructUpdateBalanceMessage(mobileNumber, threshold);

            await _clickatellBroker.SendSms(message);
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

        private static ClickatellTemplate ConstructWelcomeMessage(string mobileNumber, string organisation, string url, DateTime expireAt)
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

        private static ClickatellTemplate ConstructUpdateBalanceMessage(string mobileNumber, int threshold)
        {
            var recipient = new[]
            {
                mobileNumber
            };

            return new ClickatellTemplate()
            {
                To = recipient,
                Content = string.Format(DefinitionConstants.SmsStrings[SmsType.UpdateBalance], threshold.ToString())
            };
        }

        public async Task<Response> VerifyBalance()
        {
            var balanceThreshold = _configuration.GetValue<int>("PlatformSettings:BalanceMinThreshold");

            var balanceResponse = await _clickatellBroker.GetBalance();

            if (balanceResponse.Balance < balanceThreshold)
                await SendBalanceSms();

            return new Response(balanceResponse, HttpStatusCode.OK);
        }

        public Response CreateBalanceJob()
        {
            RecurringJob.AddOrUpdate("recurring-balance-check",() => VerifyBalance(),
                Cron.Daily);

            return new Response(true, HttpStatusCode.Created);
        }

        public Response DeleteBalanceJob()
        {
            RecurringJob.RemoveIfExists("recurring-balance-check");

            return new Response(true, HttpStatusCode.Created);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Organisation;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using Newtonsoft.Json;

namespace CoviIDApiCore.V1.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationAccessLogRepository _organisationAccessLogRepository;
        private readonly IEmailService _emailService;
        private readonly IQRCodeService _qrCodeService;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletService _walletService;

        public OrganisationService(IOrganisationRepository organisationRepository, IOrganisationAccessLogRepository organisationAccessLogRepository,
            IEmailService emailService, IQRCodeService qrCodeService, IWalletRepository walletRepository, IWalletService walletService)
        {
            _organisationRepository = organisationRepository;
            _organisationAccessLogRepository = organisationAccessLogRepository;
            _emailService = emailService;
            _qrCodeService = qrCodeService;
            _walletRepository = walletRepository;
            _walletService = walletService;
        }

        public async Task CreateAsync(CreateOrganisationRequest payload)
        {
            var companyNameRef = payload.FormResponse.Definition.Fields
                .FirstOrDefault(t => string.Equals(t.Title, DefinitionConstants.CompanyName, StringComparison.Ordinal))?
                .Reference;

            var companyName = payload.FormResponse.Answers
                .FirstOrDefault(t => string.Equals(t.Field.Reference, companyNameRef, StringComparison.Ordinal))?
                .Text;

            var organisation = new Organisation()
            {
                Name = companyName,
                Payload = JsonConvert.SerializeObject(payload),
                CreatedAt = DateTime.UtcNow
            };

            await _organisationRepository.AddAsync(organisation);

            await _organisationRepository.SaveAsync();

            await NotifyOrganisation(companyName, payload, organisation);
        }

        public async Task<Response> GetAsync(string id)
        {
            var organisation = await _organisationRepository.GetAsync(Guid.Parse(id));

            if (organisation == default)
                return new Response(false, HttpStatusCode.NotFound, Messages.Org_NotExists);

            var accessLogs = await _organisationAccessLogRepository.GetByCurrentDayByOrganisation(organisation);

            var orgCounter = accessLogs
                .Where(oal => oal.Organisation == organisation)
                .Where(oal => oal.CreatedAt.Date == DateTime.UtcNow.Date)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();

            var totalScans = accessLogs.Count(oal => oal.Organisation == organisation && oal.CreatedAt.Date == DateTime.UtcNow.Date);

            return new Response(new OrganisationDTO(organisation, orgCounter, totalScans, GetAccessLogBalance(accessLogs)), HttpStatusCode.OK);
        }

        public async Task<Response> UpdateCountAsync(string id, UpdateCountRequest payload, ScanType scanType)
        {
            Wallet wallet = null;

            if (!string.IsNullOrEmpty(payload.WalletId))
            {
                wallet = await _walletRepository.GetAsync(Guid.Parse(payload.WalletId));

                if (wallet == null)
                    throw new NotFoundException(Messages.Wallet_NotFound);
            }

            var organisation = await _organisationRepository.GetWithLogsAsync(Guid.Parse(id));

            if (organisation == default)
                throw new NotFoundException(Messages.Org_NotExists);

            var newCount = new OrganisationAccessLog()
            {
                Wallet = wallet,
                Organisation = organisation,
                CreatedAt = DateTime.UtcNow,
                ScanType = scanType
            };

            await _organisationAccessLogRepository.AddAsync(newCount);

            await _organisationAccessLogRepository.SaveAsync();

            var logs = organisation.AccessLogs
                .Where(oal => oal.CreatedAt.Date.Equals(DateTime.UtcNow.Date))
                .ToList();

            return new Response(
                new UpdateCountResponse()
                {
                    Balance = logs.Count == 0 ? 0 : GetAccessLogBalance(logs)
                },
                true,
                HttpStatusCode.OK);
        }

        public async Task<Response> MobileEntry(string organisationId, MobileEntryRequest payload)
        {
            var wallet = await _walletService.CreateWalletAndSms(payload.WalletRequest);

            var updateCounterRequest = new UpdateCountRequest
            {
                Latitude = payload.Latitude,
                Longitude = payload.Longitude,
                WalletId = wallet.Id.ToString()
            };

            var counterResponse = await UpdateCountAsync(organisationId, updateCounterRequest, ScanType.CheckIn);
            return counterResponse;
        }

        #region Private Methods
        private int GetAccessLogBalance(List<OrganisationAccessLog> logs)
        {
            var checkIns = logs.Count(oal => oal.ScanType == ScanType.CheckIn);
            var checkOuts = logs.Count(oal => oal.ScanType == ScanType.CheckOut);

            return checkIns - checkOuts; //TODO: Maybe improve this?
        }

        private async Task NotifyOrganisation(string companyName, CreateOrganisationRequest payload, Organisation organisation)
        {
            var emailAddressRef = payload.FormResponse.Definition.Fields
                .FirstOrDefault(t => string.Equals(t.Title, DefinitionConstants.EmailAdress, StringComparison.Ordinal))?
                .Reference;

            var emailAddress = payload.FormResponse.Answers
                .FirstOrDefault(t => string.Equals(t.Field.Reference, emailAddressRef, StringComparison.Ordinal))?
                .Email;

            if (string.IsNullOrEmpty(emailAddress))
                throw new ValidationException(Messages.Org_EmailEmpty);

            await _emailService.SendEmail(
                emailAddress,
                companyName,
                _qrCodeService.GenerateQRCode(organisation.Id.ToString()),
                DefinitionConstants.EmailTemplates.OrganisationWelcome);
        }
        #endregion
    }
}
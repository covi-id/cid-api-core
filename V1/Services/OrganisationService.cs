using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Organisation;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.DTOs.Wallet;
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
        private readonly ISessionService _sessionService;
        private readonly ISmsService _smsService;
        private readonly ICryptoService _cryptoService;

        public OrganisationService(IOrganisationRepository organisationRepository, IOrganisationAccessLogRepository organisationAccessLogRepository,
            IEmailService emailService, IQRCodeService qrCodeService, IWalletRepository walletRepository, IWalletService walletService,
            ISessionService sessionService, ISmsService smsService, ICryptoService cryptoService)
        {
            _organisationRepository = organisationRepository;
            _organisationAccessLogRepository = organisationAccessLogRepository;
            _emailService = emailService;
            _qrCodeService = qrCodeService;
            _walletRepository = walletRepository;
            _walletService = walletService;
            _sessionService = sessionService;
            _smsService = smsService;
            _cryptoService = cryptoService;
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

        public async Task<Response> UpdateCountAsync(string id, UpdateCountRequest payload, ScanType scanType, bool mobile = false)
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

            await ValidateScan(organisation.AccessLogs.ToList(), scanType, wallet, mobile);

            await UpdateLogs(wallet, organisation, scanType);

            var logs = organisation.AccessLogs
                .Where(oal => oal.CreatedAt.Date.Equals(DateTime.UtcNow.Date))
                .ToList();

            return new Response(
                new UpdateCountResponse()
                {
                    Balance = logs.Count == 0 ? 0 : GetAccessLogBalance(logs),
                    Total = logs.Count == 0 ? 0 : GetAccessLogTotal(logs)
                },
                true,
                HttpStatusCode.OK);
        }

        private async Task UpdateLogs(Wallet wallet, Organisation organisation, ScanType scanType)
        {
            var newCount = new OrganisationAccessLog()
            {
                Wallet = wallet,
                Organisation = organisation,
                CreatedAt = DateTime.UtcNow,
                ScanType = scanType
            };

            await _organisationAccessLogRepository.AddAsync(newCount);

            await _organisationAccessLogRepository.SaveAsync();
        }

        private async Task ValidateScan(List<OrganisationAccessLog> logs, ScanType scanType, Wallet wallet, bool mobile = false)
        {
            if (wallet != default && !mobile)
            {
                var userLogs = logs
                    .Where(l => l.Wallet == wallet && l.CreatedAt.Date == DateTime.Now.Date)
                    .OrderByDescending(l => l.CreatedAt)
                    .ToList();

                if(!userLogs.Any() && scanType == ScanType.CheckOut)
                    throw new ValidationException(Messages.Org_UserNotScannedIn);

                if (userLogs.FirstOrDefault()?.ScanType == ScanType.CheckIn && scanType == ScanType.CheckIn)
                    await UpdateLogs(wallet, userLogs.FirstOrDefault()?.Organisation, ScanType.CheckOut);

                if(!userLogs.Any(l => l.ScanType == ScanType.CheckIn) && scanType == ScanType.CheckOut)
                    throw new ValidationException(Messages.Org_UserNotScannedIn);

                if(userLogs.FirstOrDefault()?.ScanType == ScanType.CheckOut && scanType == ScanType.CheckOut)
                    throw new ValidationException(Messages.Org_UserScannedOut);
            }

            var balance = GetAccessLogBalance(logs);

            if(balance < 1 && scanType == ScanType.CheckOut)
                throw new ValidationException(Messages.Org_NegBalance);
        }

        public async Task<Response> MobileCheckIn(string organisationId, MobileUpdateCountRequest payload)
        {
            var walletRequest = new CreateWalletRequest
            {
                MobileNumber = payload.MobileNumber
            };

            var wallet = await _walletService.CreateWallet(walletRequest, true);

            var session = await _sessionService.CreateSession(payload.MobileNumber, wallet);

            var organisation = await _organisationRepository.GetAsync(Guid.Parse(organisationId));
            if (organisation == default)
                throw new NotFoundException(Messages.Org_NotExists);

            await _smsService.SendWelcomeSms(payload.MobileNumber, organisation.Name, session.ExpireAt, session.Id);

            var updateCounterRequest = new UpdateCountRequest
            {
                Latitude = payload.Latitude,
                Longitude = payload.Longitude,
                WalletId = wallet.Id.ToString()
            };

            var counterResponse = await UpdateCountAsync(organisationId, updateCounterRequest, ScanType.CheckIn, true);

            return counterResponse;
        }

        public async Task<Response> MobileCheckOut(string organisationId, MobileUpdateCountRequest payload)
        {
            _cryptoService.EncryptAsServer(payload, true);

            var updateCounterRequest = new UpdateCountRequest
            {
                Latitude = payload.Latitude,
                Longitude = payload.Longitude,
                WalletId = await GetCheckoutWalletId(payload.MobileNumber)
            };

            var counterResponse = await UpdateCountAsync(organisationId, updateCounterRequest, ScanType.CheckOut, true);

            return counterResponse;
        }

        #region Private Methods
        private async Task<string> GetCheckoutWalletId(string mobileNumber)
        {
            var wallets = await _walletRepository.GetListByEncryptedMobileNumber(mobileNumber);

            if (wallets == default)
                throw new ValidationException(Messages.Wallet_NotFound);

            var organisationAccessLogs = await _organisationAccessLogRepository
                .GetListByWalletIds(wallets.Select(w => w.Id).ToList());

            organisationAccessLogs.RemoveAll(t =>
                organisationAccessLogs
                .Where(oal => oal.ScanType == ScanType.CheckOut)
                .Select(oal => oal.Wallet.Id)
                .Distinct()
                .ToList()
                .Contains(t.Wallet.Id));

            var log = organisationAccessLogs.FirstOrDefault();

            if(log == null)
                throw new NotFoundException();

            var wallet = wallets.FirstOrDefault(w => Equals(w.Id, log.Wallet.Id));

            if(wallet == null)
                throw new NotFoundException(Messages.Wallet_NotFound);

            return wallet.Id.ToString();
        }

        private int GetAccessLogBalance(List<OrganisationAccessLog> logs)
        {
            var checkIns = logs.Count(oal => oal.ScanType == ScanType.CheckIn);
            var checkOuts = logs.Count(oal => oal.ScanType == ScanType.CheckOut);

            return checkIns - checkOuts;
        }

        private int GetAccessLogTotal(List<OrganisationAccessLog> logs)
        {
            return logs.Count(oal => oal.ScanType == ScanType.CheckIn);
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
using System;
using CoviIDApiCore.V1.DTOs.Connection;
using CoviIDApiCore.V1.DTOs.Credentials;
using CoviIDApiCore.V1.DTOs.Wallet;
using CoviIDApiCore.V1.Interfaces.Brokers;
using CoviIDApiCore.V1.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using static CoviIDApiCore.V1.Constants.DefinitionConstants;
using CoviIDApiCore.Models.AppSettings;

namespace CoviIDApiCore.V1.Services
{
    public class WalletService : IWalletService
    {
        private readonly ICustodianBroker _custodianBroker;
        private readonly IAgencyBroker _agencyBroker;
        private readonly IConnectionService _connectionService;
        private readonly ICredentialService _credentialService;
        private readonly IConfiguration _configuration;
        private readonly IOtpService _otpService;
        private readonly IWalletRepository _walletRepository;
        private readonly StreetcredDefinitions _streetcredDefinitions;

        public WalletService(ICustodianBroker custodianBroker, IConnectionService connectionService, IAgencyBroker agencyBroker,
            IConfiguration configuration, IOtpService otpService, IWalletRepository walletRepository,
            StreetcredDefinitions streetcredDefinitions,
            ICredentialService credentialService)
        {
            _custodianBroker = custodianBroker;
            _connectionService = connectionService;
            _agencyBroker = agencyBroker;
            _configuration = configuration;
            _credentialService = credentialService;
            _otpService = otpService;
            _walletRepository = walletRepository;
            _streetcredDefinitions = streetcredDefinitions;
        }

        public async Task<List<WalletContract>> GetWallets()
        {
            return await _custodianBroker.GetWallets();
        }

        public async Task<WalletContract> CreateWallet(WalletParameters walletParameters)
        {
            return await _custodianBroker.CreateWallet(walletParameters);
        }

        public async Task<CoviIdWalletContract> CreateCoviIdWallet(CoviIdWalletParameters coviIdWalletParameters)
        {
            var wallet = new WalletParameters
            {
                OwnerName = $"{coviIdWalletParameters.FirstName}-{coviIdWalletParameters.LastName}"
            };

            var response = await _custodianBroker.CreateWallet(wallet);

            var pictureUrl = await _agencyBroker.UploadFiles(coviIdWalletParameters.Photo, response.WalletId);

            var newWallet = await SaveNewWalletAsync(response.WalletId);

            await _otpService.GenerateAndSendOtpAsync(coviIdWalletParameters.MobileNumber.ToString(), newWallet);
            
            var contract = new CoviIdWalletContract
            {
                CovidStatusUrl = $"{_configuration.GetValue<string>("CoviIDBaseUrl")}/api/verifier/{response.WalletId}/covid-credentials",
                Picture = pictureUrl,
                WalletId = response.WalletId
            };

            return contract;
        }

        private async Task<Wallet> SaveNewWalletAsync(string walletId)
        {
            var newWallet = new Wallet()
            {
                WalletIdentifier = walletId,
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.AddAsync(newWallet);

            await _walletRepository.SaveAsync();

            return newWallet;
        }

        /// <summary>
        /// This will update the wallet with the newly added covid test results
        /// </summary>
        /// <returns></returns>
        public async Task UpdateWallet(CovidTestCredentialParameters covidTest, string walletId)
        {
            var connectionParameters = new ConnectionParameters
            {
                ConnectionId = "", // Leave blank for auto generation
                Multiparty = false,
                Name = _streetcredDefinitions.TenantName
            };

            var agentInvitation = await _connectionService.CreateInvitation(connectionParameters);
            var custodianConnection = await _connectionService.AcceptInvitation(agentInvitation.Invitation, walletId);
            var offer = await _credentialService.CreateCovidTest("", covidTest, walletId);
            var userCredentials = await _custodianBroker.GetCredentials(walletId);

            var thisOffer = userCredentials.FirstOrDefault(c => c.State == CredentialsState.Offered && c.DefinitionId == _streetcredDefinitions.DefinitionIds[Schema.CovidTest]);
            if (thisOffer != null)
            {
                await _custodianBroker.AcceptCredential(walletId, thisOffer.CredentialId);

            }
            // TODO : Throw exception

            return;
        }

        public async Task DeleteWallet(string walletId)
        {
            await _custodianBroker.DeleteWallet(walletId);
        }

        public async Task DeleteWallets(List<WalletParameters> wallets)
        {
            foreach (var wallet in wallets)
            {
                await _custodianBroker.DeleteWallet(wallet.WalletId);
            }
        }
    }
}
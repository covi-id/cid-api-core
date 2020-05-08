using CoviIDApiCore.V1.DTOs.Verify;
using CoviIDApiCore.V1.Interfaces.Services;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;

namespace CoviIDApiCore.V1.Services
{
    public class VerifyService : IVerifyService
    {
        private readonly IOrganisationService _organisationService;
        private readonly ICredentialService _credentialService;

        public VerifyService(IOrganisationService organisationService, ICredentialService credentialService)
        {
            _organisationService = organisationService;
            _credentialService = credentialService;
        }

        public async Task<VerifyResult> GetCredentials(string walletId, string organisationId, string deviceIdentifier)
        {
            var coviIdCredentials = await _credentialService.GetCoviIDCredentials(walletId);

            if (coviIdCredentials.CovidTestCredentials == default)
                throw new NotFoundException(Messages.Ver_CoviIDNotFound);

            var covidStatus = coviIdCredentials.CovidTestCredentials.CovidStatus;

            return new VerifyResult
            {
                Picture = coviIdCredentials.PersonCredentials.Photo,
                Name = coviIdCredentials.PersonCredentials.FirstName,
                Surname = coviIdCredentials.PersonCredentials.LastName,
                Status = (int)coviIdCredentials.CovidTestCredentials.CovidStatus,
                CovidStatus = covidStatus.ToString()
            };
        }
    }
}

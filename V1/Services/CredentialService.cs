using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Credentials;
using CoviIDApiCore.V1.Interfaces.Brokers;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static CoviIDApiCore.V1.Constants.DefinitionConstants;

namespace CoviIDApiCore.V1.Services
{
    /// <summary>
    /// This class ensures that the credentials are always created with the correct values and associated Definition ID. 
    /// This has context of the correct Definition ID and does not require a lookup for the ID's.
    /// </summary>
    public class CredentialService : ICredentialService
    {
        private readonly ICustodianBroker _custodianBroker;

        public CredentialService(ICustodianBroker custodianBroker)
        {
            _custodianBroker = custodianBroker;
        }

        public async Task<CoviIDCredentialContract> GetCoviIDCredentials(string walletId)
        {
            var covidTestCredentials = new CovidTestCredentialParameters();

            var allCredentials = await _custodianBroker.GetCredentials(walletId);
            if (allCredentials == null)
                throw new ValidationException(Messages.Cred_NotFound);

            var offeredCredentials = allCredentials?.Where(x => x.State == CredentialsState.Requested).ToList();
            if (offeredCredentials == null)
                throw new ValidationException(Messages.Cred_RequestedNotFound);


            var verifiedPerson = offeredCredentials?.FirstOrDefault(p => p.DefinitionId == DefinitionIds[Schemas.Person]);
            var covidTest = offeredCredentials?.Where(c => c.DefinitionId == DefinitionIds[Schemas.CovidTest])
                .OrderBy(c => c.Values.TryGetValue(SchemaAttributes.DateIssued, out var dateIssued)).ToList().FirstOrDefault();

            if (verifiedPerson != null)
            {
                //TODO: Optimize
                verifiedPerson.Values.TryGetValue(SchemaAttributes.FirstName, out string firstName);
                verifiedPerson.Values.TryGetValue(SchemaAttributes.LastName, out string lastName);
                verifiedPerson.Values.TryGetValue(SchemaAttributes.Photo, out string photo);
                verifiedPerson.Values.TryGetValue(SchemaAttributes.MobileNumber, out string mobileNumber);
                verifiedPerson.Values.TryGetValue(SchemaAttributes.IdentificationType, out string identificationTypeStr);
                verifiedPerson.Values.TryGetValue(SchemaAttributes.IdentificationValue, out string identificationValue);

                var identificationType = (IdentificationTypes)Enum.Parse(typeof(IdentificationTypes), identificationTypeStr);

                if (covidTest != null)
                {
                    covidTest.Values.TryGetValue(SchemaAttributes.ReferenceNumber, out string referenceNumber);
                    covidTest.Values.TryGetValue(SchemaAttributes.Laboratory, out string laboratoryStr);
                    covidTest.Values.TryGetValue(SchemaAttributes.DateTested, out string dateTested);
                    covidTest.Values.TryGetValue(SchemaAttributes.DateIssued, out string dateIssued);
                    covidTest.Values.TryGetValue(SchemaAttributes.CovidStatus, out string covidStatusStr);


                    var laboratory = (Laboratory)Enum.Parse(typeof(Laboratory), laboratoryStr);
                    var covidStatus = (CovidStatus)Enum.Parse(typeof(CovidStatus), covidStatusStr);

                    covidTestCredentials.DateIssued = DateTime.Parse(dateIssued);
                    covidTestCredentials.DateTested = DateTime.Parse(dateTested);
                    covidTestCredentials.Laboratory = laboratory;
                    covidTestCredentials.ReferenceNumber = referenceNumber;
                    covidTestCredentials.CovidStatus = covidStatus;
                }

                return new CoviIDCredentialContract
                {
                    CovidTestCredentials = covidTestCredentials,
                    PersonCredentials = new PersonCredentialParameters
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        IdentificationType = identificationType,
                        IdentificationValue = identificationValue,
                        MobileNumber = long.Parse(mobileNumber),
                        Photo = photo
                    }
                };
            }
            throw new ValidationException(Messages.Cred_VerifidPersonNotFound);
        }
    }
}

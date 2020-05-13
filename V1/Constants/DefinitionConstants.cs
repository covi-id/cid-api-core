using System.Collections.Generic;

namespace CoviIDApiCore.V1.Constants
{
    public class DefinitionConstants
    {
        #region Strings
        public static readonly string CompanyName = "Company Name";
        public static readonly string EmailAdress = "Email Address";
        public static readonly string AgentName = "CoviID";
        #endregion

        public class SchemaAttributes
        {
            // Covid Test Schema
            public static readonly string ReferenceNumber = "testReferenceNumber";
            public static readonly string Laboratory = "issuedLaboratory";
            public static readonly string DateTested = "dateTested";
            public static readonly string DateIssued = "dateIssued";
            public static readonly string CovidStatus = "covidStatus";
            // Verifier Person Schema
            public static readonly string FirstName = "firstName";
            public static readonly string LastName = "lastName";
            public static readonly string Photo = "photo";
            public static readonly string MobileNumber = "mobileNumber";
            public static readonly string IdentificationType = "identificationType";
            public static readonly string IdentificationValue = "identificationValue";
        }

        public static Dictionary<EmailTemplates, string> TemplateIds = new Dictionary<EmailTemplates, string>
        {
            { EmailTemplates.OrganisationWelcome, "d-5ceb0422ddfd4850b361255bcc30fde2" }
        };

        public static Dictionary<EmailTemplates, string> EmailSubjects = new Dictionary<EmailTemplates, string>
        {
            { EmailTemplates.OrganisationWelcome, "Welcome to the Covi-ID platform!" }
        };

        public enum EmailTemplates
        {
            OrganisationWelcome
        }

        /// <summary>
        /// Pass in the selected schema and retrieve the corresponding definition Id
        /// This is added here to save costs to Streetcred API
        /// </summary>
        public static Dictionary<Schemas, string> DefinitionIds = new Dictionary<Schemas, string>
        {
            { Schemas.Person, "RYJoQ8UNadGrrfL7PBK8Wm:3:CL:95145:Verified Person" },
            { Schemas.CovidTest, "RYJoQ8UNadGrrfL7PBK8Wm:3:CL:94574:Covid Test" }
        };

        public enum Schemas
        {
            Person,
            CovidTest
        }

        public static Dictionary<IdentityClaims, string> IdentityClaimStrings = new Dictionary<IdentityClaims, string>()
        {
            { IdentityClaims.UniqueName, "unique_name" },
            { IdentityClaims.Sid, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid"}
        };

        public enum IdentityClaims
        {
            UniqueName,
            Sid
        }

        public static Dictionary<SmsType, string> SmsStrings = new Dictionary<SmsType, string>()
        {
            { SmsType.Otp, "Your OTP: {0}. This OTP will expire in {1} minutes." },
            { SmsType.Welcome, "A Covi-ID was generated for you at {0} today. Navigate to {1} to download and use going forward."}
        };

        public enum SmsType
        {
            Otp,
            Welcome
        }
    }
}

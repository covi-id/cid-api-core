using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoviIDApiCore.V1.Constants
{
    public class DefinitionConstants
    {
        #region Strings
        public static readonly string CompanyName = "Company Name";
        public static readonly string EmailAdress = "Email Address";
        #endregion

        public class Attributes
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

        public enum Schema
        {
            [EnumMember(Value = "Person")]
            Person = 0,
            [EnumMember(Value = "CovidTest")]
            CovidTest = 1
        }
    }

    public enum UpdateType
    {
        Addition,
        Subtraction
    }
}

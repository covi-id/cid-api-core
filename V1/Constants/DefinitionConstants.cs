using System.Collections.Generic;

namespace CoviIDApiCore.V1.Constants
{
    public class DefinitionConstants
    {
        #region Strings
        public static readonly string CompanyName = "Company Name";
        public static readonly string EmailAdress = "Email Address";
        #endregion

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
            { SmsType.Welcome, "A Covi-ID was generated for you today at {0}. Navigate to {1} to download and use going forward. This link will expire at {2}"},
            { SmsType.UpdateBalance, "Covi-ID account balance below R{0}. Please update balance."}
        };

        public enum SmsType
        {
            Otp,
            Welcome,
            UpdateBalance
        }
    }
}

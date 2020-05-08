namespace CoviIDApiCore.V1.Constants
{
    public class Messages
    {
        #region Misc
        public static readonly string Misc_Success = "Success.";
        public static readonly string Misc_SomethingWentWrong = "Oops! Something went wrong. Please try again later.";
        public static readonly string Misc_ThirdParty = "Third party error.";
        public static readonly string Misc_Unauthorized = $"Unauthorised.";
        #endregion

        #region DbErrors
        public static readonly string FailedToSave = "Failed to save entry(s) to database. See inner exception for details.";
        public static readonly string CreateNull = "Object is null whilst attempting to add to the database.";
        public static readonly string AddRangeNull = "Object list is null whilst attempting to add entries to the database.";
        public static readonly string DeleteNull = "Object is null whilst attempting to delete from the database.";
        public static readonly string GetNull = "Object is null whilst attempting to get from the database.";
        public static readonly string InvalidPageInfo = "Invalid page information. Page number and size must be greater than 0";
        #endregion

        #region Organisations
        public static readonly string Org_NotExists = $"Organisation does not exist.";
        public static readonly string Org_PayloadInvalid = $"Payload is invalid.";
        public static readonly string Org_NegBalance = $"Balance can not be less than 0.";
        public static readonly string Org_EmailEmpty = $"Organisation email is empty.";
        #endregion

        #region QRCode
        public static readonly string QR_Failed = $"QR code generation failed.";
        #endregion

        #region Tokens
        public static readonly string Token_OTPNotExist = $"OTP could not be found.";
        public static readonly string Token_OTPExpired = $"OTP has expired.";
        public static readonly string Token_OTPFailed = $"OTP is invalid.";
        public static readonly string Token_OTPThreshold = $"Too many OTPs."; //TODO: Copy change
        public static readonly string Token_InvaldPayload = $"OTP Payload is invalid."; //TODO: Copy change
        public static readonly string Token_Invalid = $"OTP session token is invalid";
        #endregion

        #region Credentials
        public static readonly string Cred_NotFound = $"Could not find any credentials for this wallet.";
        public static readonly string Cred_OfferedNotFound = $"Could not find any offered credentials for this wallet.";
        public static readonly string Cred_RequestedNotFound = $"Could not find any Requested credentials for this wallet.";
        public static readonly string Cred_VerifidPersonNotFound = $"Could not find the verified person credentials.";
        #endregion

        #region Validation
        public static readonly string Val_Length = $"Invalid length.";
        public static readonly string Val_MobileNumber = $"Invalid mobile number.";
        public static readonly string Val_Identification = $"Invalid Identification number.";
        public static readonly string Val_DateNotInPast = $"Invalid Date, date must be in the past";
        public static readonly string Val_FileTooLarge = $"The File size is to large.";
        #endregion

        #region Verifier
        public static readonly string Ver_CoviIDNotFound = $"Could not find the CoviID for the wallet";
        #endregion

        #region Wallet
        public static readonly string Wallet_NotFound = $"Could not find wallet.";
        #endregion

        #region TestResults
        public static readonly string TestResult_Invalid = $"Test result is invalid."; //TODO: Copy change
        #endregion
    }
}
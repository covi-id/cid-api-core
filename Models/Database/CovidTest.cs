using CoviIDApiCore.V1.DTOs.Credentials;
using System;


namespace CoviIDApiCore.Models.Database
{
    public class CovidTest : BaseModel<Guid>
    {
        public string WalletId { get; set; }
        /// <summary>
        /// When the user granted permission to store the data
        /// </summary>
        public DateTime PermissionGrantedAt { get; set; }
        /// <summary>
        /// Idicates the credential progress
        /// </summary>
        public CredentialIndicator CredentialIndicator { get; set; }
        public DateTime DateTested { get; set; }
        internal DateTime DateIssued { get; set; }
        public CovidStatus CovidStatus { get; set; }
        public Laboratory Laboratory { get; set; }
        public string ReferenceNumber { get; set; }
        /// <summary>
        /// Indicates if we have consent to store the data
        /// </summary>
        public bool HasConsent { get; set; }
        public CovidTest()
        {
        }
    }
    public enum CredentialIndicator
    {
        /// <summary>
        /// Credentials have only been added to the wallet
        /// </summary>
        Added = 0,
        /// <summary>
        /// Credentials have been verified at a lab
        /// </summary>
        Verified = 1
    }
}

using CoviIDApiCore.V1.Attributes;
using System;
using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.Models.Database
{
    public class WalletDetail : BaseModel<Guid>
    {
        public virtual Wallet Wallet { get; set; }
        [Encrypted]
        public string FirstName { get; set; }
        [Encrypted]
        public string LastName { get; set; }
        [Encrypted(true)]
        public string MobileNumber { get; set; }
        public bool isMyMobileNumber { get; set; }
        [Encrypted]
        public string PhotoReference { get; set; }
        public bool HasConsent { get; set; }
        public DateTime? CreatedAt { get; set; }
        public WalletDetail()
        {
        }

        public WalletDetail(WalletDetailsRequest request)
        {
            FirstName = request.FirstName;
            LastName = request.LastName;
            MobileNumber = request.MobileNumber;
            isMyMobileNumber = request.isMyMobileNumber;
            PhotoReference = request.Photo;
            HasConsent = request.HasConsent;
            CreatedAt = DateTime.UtcNow;
        }
    }
}

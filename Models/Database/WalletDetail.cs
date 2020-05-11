using CoviIDApiCore.V1.Attributes;
using CoviIDApiCore.V1.DTOs.Wallet;
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
        public string PhotoReference { get; set; }
        public IdType IdType { get; set; }
        [Encrypted]
        public string IdValue { get; set; }

        public WalletDetail()
        {
        }

        public WalletDetail(WalletDetailsRequest detailsRequest)
        {
            FirstName = detailsRequest.FirstName;
            LastName = detailsRequest.LastName;
            PhotoReference = detailsRequest.Photo;
            IdType = detailsRequest.IdType;
            IdValue = detailsRequest.IdValue;
        }
    }
}

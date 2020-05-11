using CoviIDApiCore.V1.Attributes;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Wallet;
using System;
using CoviIDApiCore.V1.DTOs.Authentication;
using System.ComponentModel.DataAnnotations;

namespace CoviIDApiCore.Models.Database
{
    public class WalletDetail : BaseModel<Guid>
    {
        public virtual Wallet Wallet { get; set; }
        [Encrypted]
        //[StringLength(50, MinimumLength = 2, ErrorMessage = "Invalid lenght. Minimum length is 2 and maximum is 50")]
        public string FirstName { get; set; }
        [Encrypted]
        //[StringLength(50, MinimumLength = 2, ErrorMessage = "Invalid lenght. Minimum length is 2 and maximum is 50")]
        public string LastName { get; set; }
        [Encrypted]	
        //[RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Encrypted]
        public string PhotoUrl { get; set; }
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
            Email = detailsRequest.Email;
            PhotoUrl = detailsRequest.Photo;
            IdType = detailsRequest.IdType;
            IdValue = detailsRequest.IdValue;
        }
    }
}

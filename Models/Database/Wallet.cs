using CoviIDApiCore.V1.Attributes;
using System;

namespace CoviIDApiCore.Models.Database
{
    public class Wallet : BaseModel<Guid>
    {
        [Encrypted(true)]
        public string MobileNumber { get; set; }
        [Encrypted(true)]
        public string MobileNumberReference { get; set; }
        public DateTime MobileNumberVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
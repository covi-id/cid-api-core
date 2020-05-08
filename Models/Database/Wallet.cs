using CoviIDApiCore.V1.Attributes;
using System;
using System.Collections.Generic;

namespace CoviIDApiCore.Models.Database
{
    public class Wallet : BaseModel<Guid>
    {
        public string SessionId { get; set; }
        [Encrypted(true)]
        public string MobileNumber { get; set; }
        [Encrypted(true)]
        public string MobileNumberReference { get; set; }
        public DateTime MobileNumberVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
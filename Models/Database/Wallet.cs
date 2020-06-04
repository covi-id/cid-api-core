using System;

namespace CoviIDApiCore.Models.Database
{
    public class Wallet : BaseModel<Guid>
    {
        public DateTime? MobileNumberVerifiedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
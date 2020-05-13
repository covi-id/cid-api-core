using System;

namespace CoviIDApiCore.Models.Database
{
    public class Session : BaseModel<Guid>
    {
        public virtual Wallet Wallet { get; set; }
        public DateTime ExpireAt { get; set; }
        public bool isUsed { get; set; }
    }
}

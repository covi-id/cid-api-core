using System;

namespace CoviIDApiCore.Models.Database
{
    public class WalletLocationReceipt: BaseModel<Guid>
    {
        public WalletLocationReceipt()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public virtual Wallet Wallet { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public ScanType ScanType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

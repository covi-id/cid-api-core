using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoviIDApiCore.Models.Database
{
    public class WalletLocationReceipt: BaseModel<Guid>
    {
        public WalletLocationReceipt()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public virtual Wallet Wallet { get; set; }
        [Column(TypeName = "decimal(12,8)")]
        public decimal Longitude { get; set; }
        [Column(TypeName = "decimal(12,8)")]
        public decimal Latitude { get; set; }
        public ScanType ScanType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

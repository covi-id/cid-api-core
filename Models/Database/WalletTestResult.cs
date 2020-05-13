using CoviIDApiCore.V1.DTOs.WalletTestResult;
using System;

namespace CoviIDApiCore.Models.Database
{
    public class WalletTestResult : BaseModel<Guid>
    {
        public virtual Wallet Wallet { get; set; }
        public TestType TestType { get; set; }
        public LaboratoryStatus LaboratoryStatus { get; set; }
        public ResultStatus ResultStatus { get; set; }
        public Laboratory Laboratory { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime TestedAt { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime PermissionGrantedAt { get; set; }
        public bool HasConsent { get; set; }
    }
}

using CoviIDApiCore.V1.Attributes;
using CoviIDApiCore.V1.DTOs.WalletTestResult;
using System;

namespace CoviIDApiCore.Models.Database
{
    public class WalletTestResult : BaseModel<Guid>
    {
        public virtual Wallet Wallet { get; set; }
        public TestType TestType { get; set; }
        public LaboratoryStatus LaboratoryStatus { get; set; }
        public bool HasReceivedResults { get; set; }
        public ResultStatus ResultStatus { get; set; }
        public Laboratory Laboratory { get; set; }
        public DateTime? TestedAt { get; set; }
        public DateTime? IssuedAt { get; set; }
        [Encrypted]
        public string ReferenceNumber { get; set; }
        public bool HasConsent { get; set; }
        public DateTime? PermissionGrantedAt { get; set; }
        public DateTime? CreatedAt { get; set; }

        public WalletTestResult(TestResultRequest request, Wallet wallet)
        {
            Wallet = wallet;
            TestType = TestType.Covid19;
            Laboratory = request.Laboratory;
            HasReceivedResults = request.HasReceivedResults;
            ReferenceNumber = request.ReferenceNumber;
            TestedAt = request.TestedAt;
            ResultStatus = request.ResultStatus;
            LaboratoryStatus = LaboratoryStatus.Unsent;
            HasConsent = request.HasConsent;
            PermissionGrantedAt = request.PermissionGrantedAt;
            CreatedAt = DateTime.UtcNow;
        }

        public WalletTestResult()
        {

        }

    }
}

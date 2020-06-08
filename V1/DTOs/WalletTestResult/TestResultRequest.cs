using System;

namespace CoviIDApiCore.V1.DTOs.WalletTestResult
{
    public class TestResultRequest
    {
        public TestResultRequest()
        {
            if (HasConsent)
                PermissionGrantedAt = DateTime.UtcNow;
        }
        public Guid walletId { get; set; }
        public LaboratoryStatus LaboratoryStatus { get; set; } = LaboratoryStatus.Unsent;
        public bool HasReceivedResults { get; set; }
        public ResultStatus ResultStatus { get; set; }
        public Laboratory Laboratory { get; set; }
        public DateTime TestedAt { get; set; }
        public string ReferenceNumber { get; set; }
        public bool HasConsent { get; set; }
        internal DateTime? PermissionGrantedAt { get; set; }

        public bool isValid()
        {
            return TestedAt.Date < DateTime.UtcNow.Date;
        }
    }
}

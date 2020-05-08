using System;

namespace CoviIDApiCore.V1.DTOs.WalletTestResult
{
    public class TestResultResponse
    {
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

using System;

namespace CoviIDApiCore.Models.Database
{
    public class OtpToken : BaseModel<long>
    {
        public int Code { get; set; }
        public string MobileNumber { get; set; }
        public bool isUsed { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
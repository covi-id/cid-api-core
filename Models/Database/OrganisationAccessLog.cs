using System;

namespace CoviIDApiCore.Models.Database
{
    public class OrganisationAccessLog : BaseModel<Guid>
    {
        public virtual Organisation Organisation { get; set; }
        public ScanType ScanType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public OrganisationAccessLog()
        {
        }
    }

    public enum ScanType
    {
        CheckIn,
        CheckOut,
        Denied
    }
}
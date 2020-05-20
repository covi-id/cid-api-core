using System;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IStaySafeService
    {
        Task CaptureData(Guid walletId, DateTime twoWeeksFromDate);
    }
}

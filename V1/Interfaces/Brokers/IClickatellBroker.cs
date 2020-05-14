using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.Clickatell;

namespace CoviIDApiCore.V1.Interfaces.Brokers
{
    public interface IClickatellBroker
    {
        Task SendSms(object payload);
        Task<ClickatellResponse> GetBalance();
    }
}
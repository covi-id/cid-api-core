using System.Threading.Tasks;
using CoviIDApiCore.V1.Brokers;

namespace CoviIDApiCore.V1.Interfaces.Brokers
{
    public interface IClickatellBroker
    {
        Task SendSms(object payload);
        Task<ClickatellBroker> GetBalance();
    }
}
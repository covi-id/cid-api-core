using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Brokers
{
    public interface IBitlyBroker
    {
        Task<string> ShortenRequest(object payload);
    }
}
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.Bitly;

namespace CoviIDApiCore.V1.Interfaces.Brokers
{
    public interface IBitlyBroker
    {
        Task<BitlyResponse> ShortenRequest(object payload);
    }
}
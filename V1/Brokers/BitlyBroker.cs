using System.Threading.Tasks;
using CoviIDApiCore.V1.Interfaces.Brokers;
using Microsoft.Extensions.Configuration;

namespace CoviIDApiCore.V1.Brokers
{
    public class BitlyBroker : IBitlyBroker
    {
        private readonly IConfiguration _configuration;

        public BitlyBroker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ShortenRequest()
        {

        }
    }
}
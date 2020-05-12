using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace CoviIDApiCore.V1.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
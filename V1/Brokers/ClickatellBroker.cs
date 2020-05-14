using System.Net.Http;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.Interfaces.Brokers;
using Newtonsoft.Json;

namespace CoviIDApiCore.V1.Brokers
{
    public class ClickatellBroker : IClickatellBroker
    {
        private readonly HttpClient _httpClient;
        private static readonly string _sendPartialRoute = UrlConstants.PartialRoutes[UrlConstants.Routes.ClickatellSend];
        private static readonly string _balancePartialRoute =UrlConstants.PartialRoutes[UrlConstants.Routes.ClickatellBalance];

        public ClickatellBroker(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendSms(object payload)
        {
            var response = await _httpClient.PostAsJsonAsync(_sendPartialRoute, payload);

            if (!response.IsSuccessStatusCode)
                throw new ClickatellException(await response.Content.ReadAsStringAsync());
        }

        public async Task<ClickatellBroker> GetBalance()
        {
            var response = await _httpClient.GetAsync(_balancePartialRoute);

            if(!response.IsSuccessStatusCode)
                throw new ClickatellException(await response.Content.ReadAsStringAsync());

            return JsonConvert.DeserializeObject<ClickatellBroker>(await response.Content.ReadAsStringAsync());
        }
    }
}
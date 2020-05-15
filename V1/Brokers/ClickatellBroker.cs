using System.Net.Http;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Clickatell;
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

            await ValidateResponse(response);
        }

        public async Task<ClickatellResponse> GetBalance()
        {
            var response = await _httpClient.GetAsync(_balancePartialRoute);

            return JsonConvert.DeserializeObject<ClickatellResponse>(await (await ValidateResponse(response)).Content.ReadAsStringAsync());
        }

        private async Task<HttpResponseMessage> ValidateResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return response;

            var message = await response.Content.ReadAsStringAsync();

            throw new ClickatellException($"{message} Broker status code: {response.StatusCode}");
        }
    }
}
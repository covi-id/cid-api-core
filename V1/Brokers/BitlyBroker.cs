using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Bitly;
using CoviIDApiCore.V1.Interfaces.Brokers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CoviIDApiCore.V1.Brokers
{
    public class BitlyBroker : IBitlyBroker
    {
        private readonly HttpClient _httpClient;
        private static readonly string _partialRoot = UrlConstants.PartialRoutes[UrlConstants.Routes.ShortenUrl];

        public BitlyBroker(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ShortenRequest(object payload)
        {
            var response = await _httpClient.PostAsJsonAsync(_partialRoot, payload);

            if (!response.IsSuccessStatusCode)
                await ProcessFailureMessage(response);

            return await response.Content.ReadAsStringAsync();
        }

        private static async Task ProcessFailureMessage(HttpResponseMessage responseMessage)
        {
            var contentString = await responseMessage.Content.ReadAsStringAsync();

            var errorData = JsonConvert.DeserializeObject<BitlyErrorResponse>(contentString);

            var exceptionMessage = $"{errorData.Message}: ";

            exceptionMessage = errorData.Errors.Aggregate(exceptionMessage, (current, error) => current + (error.Message + "\n"));

            throw new BitlyException(exceptionMessage);
        }
    }
}
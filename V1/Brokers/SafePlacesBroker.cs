using CoviIDApiCore.Exceptions;
using CoviIDApiCore.Models.AppSettings;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.SafePlaces;
using CoviIDApiCore.V1.Interfaces.Brokers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Brokers
{
    public class SafePlacesBroker : ISafePlacesBroker
    {
        private readonly HttpClient _httpClient;
        private static readonly string _applicationJson = "application/json";
        private readonly SafePlacesCredentials _credentials;
        public SafePlacesBroker(HttpClient httpClient, SafePlacesCredentials credentials)
        {
            _httpClient = httpClient;
            _credentials = credentials;
            //Login(new LoginRequest
            //{
            //    Password = _credentials.Password,
            //    Username = _credentials.Username
            //});
        }

        public async Task<Redacted> AddRedacted(RedactedRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, _applicationJson);

            var response = await _httpClient.PostAsync(UrlConstants.PartialRoutes[UrlConstants.Routes.SafePlacesRedacted], content);

            return await ValidateResponse<Redacted>(response);
        }

        public async Task<List<Redacted>> GetRedacted()
        {
            var response = await _httpClient.GetAsync(UrlConstants.PartialRoutes[UrlConstants.Routes.SafePlacesRedacted]);

            return await ValidateResponse<List<Redacted>>(response);
        }


        private async Task<LoginResponse> Login(LoginRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, _applicationJson);

            var response = await _httpClient.PostAsync(UrlConstants.PartialRoutes[UrlConstants.Routes.SafePlacesLogin], content);

            return JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
        }

        private async Task<T> ValidateResponse<T>(HttpResponseMessage request)
        {
            var message = await request.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<T>>(message);

            if (response.Success)
            {
                return response.Data;
            }

            throw new SafePlacesException($"{message} Broker status code: {request.StatusCode}");
        }
    }
}

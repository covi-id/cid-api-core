using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.DTOs.Connection;
using CoviIDApiCore.V1.DTOs.Credentials;
using CoviIDApiCore.V1.Interfaces.Brokers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Verifications;
using CoviIDApiCore.V1.DTOs.VerificationPolicy;

namespace CoviIDApiCore.V1.Brokers
{
    public class AgencyBroker : IAgencyBroker
    {
        private readonly HttpClient _httpClient;

        private static readonly string partialRoot = UrlConstants.PartialRoutes[UrlConstants.Routes.Agency];
        private static readonly string applicationJson = "application/json";

        public AgencyBroker(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Common  
        public async Task<string> UploadFiles(string file, string fileName)
        {
            var base64Array = Convert.FromBase64String(file);

            var multipartContent = new MultipartFormDataContent
            {
                {
                    new ByteArrayContent(base64Array),
                    "uploadedFiles", Path.GetFileName(fileName)
                },
                {new StringContent(fileName), "filename"},
                {new StringContent("image/png"), "contentType"}
            };

            var response = await _httpClient.PostAsync($"{partialRoot}/common/upload", multipartContent);
            response = await ValidateResponse(response);

            return JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
        }

        #endregion

        #region Connections
        public async Task<ConnectionContract> CreateInvitation(ConnectionParameters connectionParameters)
        {
            var content = new StringContent(JsonConvert.SerializeObject(connectionParameters), Encoding.UTF8, applicationJson);

            var response = await _httpClient.PostAsync($"{partialRoot}connections", content);
            response = await ValidateResponse(response);

            return JsonConvert.DeserializeObject<ConnectionContract>(await response.Content.ReadAsStringAsync());
        }
        #endregion

        #region Credentials
        public async Task<CredentialsContract> GetCredential(string credentialId)
        {
            var response = await _httpClient.GetAsync($"{partialRoot}credentials?{credentialId}");
            response = await ValidateResponse(response);
            return JsonConvert.DeserializeObject<CredentialsContract>(await response.Content.ReadAsStringAsync());
        }

        public async Task<CredentialsContract> SendCredentials(CredentialOfferParameters credentials)
        {
            var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, applicationJson);

            var response = await _httpClient.PostAsync($"{partialRoot}credentials", content);
            response = await ValidateResponse(response);

            return JsonConvert.DeserializeObject<CredentialsContract>(await response.Content.ReadAsStringAsync());
        }
        #endregion

        #region Verificaitons

        public async Task<VerificationContract> SendVerification(VerificationPolicyParameters verificationPolicyParameters, string connectionId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(verificationPolicyParameters), Encoding.UTF8, applicationJson);

            var response = await _httpClient.PostAsync($"{partialRoot}/verifications/policy/connections/{connectionId}", content);
            response = await ValidateResponse(response);

            return JsonConvert.DeserializeObject<VerificationContract>(await response.Content.ReadAsStringAsync());
        }

        public async Task<VerificationContract> GetVerification(string verificationId)
        {
            var response = await _httpClient.GetAsync($"{partialRoot}/verifications/{verificationId}");
            response = await ValidateResponse(response);

            return JsonConvert.DeserializeObject<VerificationContract>(await response.Content.ReadAsStringAsync());
        }
        #endregion

        private async Task<HttpResponseMessage> ValidateResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return response;

            var message = await response.Content.ReadAsStringAsync();
            throw new StreetCredBrokerException($"{message} Broker status code: {response.StatusCode}");
        }
    }
}

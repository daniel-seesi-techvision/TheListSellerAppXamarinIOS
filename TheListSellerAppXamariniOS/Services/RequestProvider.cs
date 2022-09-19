using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheListSellerAppXamariniOS.Data.Repository;
using TheListSellerAppXamariniOS.Views.Products;

namespace TheListSellerAppXamariniOS.Services
{
    public class RequestProvider : IRequestProvider
    {
        private readonly INetworkService _networkService;
        private readonly IDataRepository<Product> DataRepository;

        public RequestProvider(INetworkService networkService, IDataRepository<Product> dataRepository)
        {
            _networkService = networkService;
            DataRepository = dataRepository;
        }


        public async Task<TResult> GetAsync<TResult>(HttpRequestModel model)
        {
            if (CanMakeRequest())
            {
                using (HttpClient httpClient = CreateClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(model.Uri)
                        .ConfigureAwait(false);

                    // Checks if request was not successful and ends the process
                    await HandleResponse(response).ConfigureAwait(false);

                    string serializedResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    TResult result = await Task.Run(() => JsonConvert.DeserializeObject<TResult>(serializedResponse)).ConfigureAwait(false);
                    return result;
                }
            }
            else
            {
                throw new HttpRequestException("Could not connect to your network, Try again");
            }
        }

        #region Internal Methods

        private HttpClient CreateClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Debugger.Break();

                    if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new HttpRequestException("Not authorized to access the endpoint");
                }
            }
            catch (JsonReaderException)
            {
                throw new HttpRequestException("Something went wrong executing your request, please try again");
            }
        }

        /// <summary>
        /// Checks the user's device for Internet availability and strength
        /// </summary>
        /// <returns></returns>
        private bool CanMakeRequest()
        {
            return (_networkService.IsWifiConnected() || _networkService.IsInternetConnected());
        }

        private string GetConnectivityErrorMessage()
        {
            if (!_networkService.IsInternetConnected() || !_networkService.IsWifiConnected())
                return "Your device does not appear to be connected to the Internet";
            else
                return "Your device appears to have Internet connectivity issues";
        }
        #endregion Internal Methods
    }
}
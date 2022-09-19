using System;
namespace TheListSellerAppXamariniOS.Services
{
    public class HttpRequestModel
    {
        public HttpRequestModel()
        {

        }

        public HttpRequestModel(string uri)
        {
            Uri = uri;
        }

        public HttpRequestModel(string uri, object data)
        {
            Uri = uri;
            Data = data;
        }
        public string Uri { get; set; }
        public object Data { get; set; }
    }
}


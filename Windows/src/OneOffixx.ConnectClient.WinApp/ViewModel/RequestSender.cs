/* =============================================================================
 * Copyright (C) by Sevitec AG
 *
 * Project: OneOffixx.ConnectClient.WinApp.ViewModel
 * 
 * =============================================================================
 * */
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OneOffixx.ConnectClient.WinApp.Model;

namespace OneOffixx.ConnectClient.WinApp.ViewModel
{
    /// <summary>
    /// Helper Class to Send Requests parallel.
    /// Its easier like this, than build the request in the Parallel.For function.
    /// </summary>
    public class RequestSender
    {
        private HttpClient client;
        private RequestModel request;

        public RequestSender(RequestModel request)
        {
            this.request = request;
            client = new HttpClient();
        }

        public Task<HttpResponseMessage> SendRequestTask()
        {
            var inBytes = Encoding.ASCII.GetBytes($"{request.Username}:{request.Password}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(inBytes));

            string requestXml = Regex.Replace(request.XmlString, @"\$\(GUID\)", Guid.NewGuid().ToString(), RegexOptions.IgnoreCase);

            HttpContent content = new StringContent(requestXml);
            return client.PostAsync(request.Url, content);
        }
    }
}
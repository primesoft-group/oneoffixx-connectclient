using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OneOffixx.ConnectClient.WinApp.Model;

namespace OneOffixx.ConnectClient.WinApp.ViewModel
{
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
            HttpContent content = new StringContent(request.XmlString);
            return client.PostAsync(request.Url, content);
        }
    }
}
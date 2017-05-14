using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using SharpMUD.Exceptions;

namespace SharpMUD.API
{
    internal class Requester
    {
        private static readonly HttpClient _client = new HttpClient();

        internal static string PostRequest(string endpoint, JObject jObject)
        {
            var content = new StringContent(JsonConvert.SerializeObject(jObject), Encoding.UTF8, "application/json");
            var response = _client.PostAsync(endpoint, content).Result;

            if(!response.IsSuccessStatusCode)
            {
                throw new RequestException("Server response not 200 OK.", response);
            }

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}

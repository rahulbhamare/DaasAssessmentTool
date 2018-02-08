using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary
{
    public class HTTPClientUtils
    {
        //HTTPClient GET api
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();
            
            return await Task.Run(()=> response);
        }


        //HTTPClient POST api
        public async Task<JsonValue> PostAsync(string uri, string data)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(uri, new StringContent(data));
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonValue.Parse(content));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpClientForTesting;

namespace ExampleApp
{
    public class Api
    {
        public async Task<string> HttpGet(string uri)
        {
#if DEBUG
            var client = new HttpClientForTesting.TestHttpClient(new TestHttpClientSettings
            {
                LoadCacheFrom = "TestData\\ThreeSampleResponses.json"
                // SaveCacheTo = "MyCacheFileName.json"
            });
#else
            var client = new HttpClient();
#endif
            return await client.GetStringAsync(uri);
        }
    }
}

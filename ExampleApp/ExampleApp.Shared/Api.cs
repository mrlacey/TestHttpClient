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
                LoadCacheFromAppxFile = "TestData\\ThreeSampleResponses.json",

                // TODO: CONFIRM-IS THIS REALLY REQUIRED????
                LoadCacheFrom = "MyCacheFileName.json",
                
                // SaveCacheTo = "MyCacheFileName.json",

                //IgnoreQueryStringParameters = "cacheBuster",
                IgnoreQueryStringParameters = new[] { "cacheBuster" },
                
                // This can accept a string or array of strings
                //FailRequestsContaining = ".com/something",
                FailRequestsContaining = new[] { ".com/something", ".com/something?else=true" },

                // This can be a string or array of strings too
                FailRequestsMatchingRegex = @".id=\d{3}."
            });
#else
            var client = new HttpClient();
#endif
            return await client.GetStringAsync(uri);
        }
    }
}

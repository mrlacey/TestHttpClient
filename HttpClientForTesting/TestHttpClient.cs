using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

namespace HttpClientForTesting
{
    public class TestHttpClient
    {
        private static Dictionary<Uri, string> cache = new Dictionary<Uri, string>();
        private readonly HttpMessageHandler _handler;
        private readonly bool _disposeHandler;
        private readonly TestHttpClientSettings _settings;
        private string cacheFilePath;

        public TestHttpClient(TestHttpClientSettings settings = null)
            : this(null, settings)
        {
        }

        public TestHttpClient(HttpMessageHandler handler, TestHttpClientSettings settings = null)
            : this(handler, false, settings)
        {
        }

        public TestHttpClient(HttpMessageHandler handler, bool disposeHandler, TestHttpClientSettings settings = null)
        {
            _handler = handler;
            _disposeHandler = disposeHandler;
            _settings = settings;

            if (settings != null)
            {
                if (!string.IsNullOrWhiteSpace(settings.LoadCacheFrom))
                {
                    try
                    {
                        var fldr = Windows.ApplicationModel.Package.Current.InstalledLocation;
                        var file = fldr.GetFileAsync(settings.LoadCacheFrom).GetResults();
                        var contents = FileIO.ReadTextAsync(file).GetResults();

                        cache = JsonConvert.DeserializeObject<Dictionary<Uri, string>>(contents);

                        Debug.WriteLine("Loaded cache file: {0}", settings.LoadCacheFrom);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Failed to load cached file '{0}'. Error: {1}", settings.LoadCacheFrom, exc.Message);
                    }
                }
            }

            if (settings != null && !string.IsNullOrWhiteSpace(settings.SaveCacheTo))
            {
                this.cacheFilePath = settings.SaveCacheTo;
            }
            else
            {
                this.cacheFilePath = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss") + ".cache.json";
            }

            Debug.WriteLine("Cache will be saved to '{0}\\{1}'", ApplicationData.Current.LocalFolder.Path, this.cacheFilePath);
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            return await GetStringAsync(new Uri(requestUri));
        }

        public async Task<string> GetStringAsync(Uri requestUri)
        {
            if (cache.ContainsKey(requestUri))
            {
                return cache[requestUri];
            }

            var client = _handler != null ? new HttpClient(_handler, _disposeHandler)
                                          : new HttpClient();

            var resp = await client.GetStringAsync(requestUri);

            cache.Add(requestUri, resp);

            await this.PersistCache();

            return resp;
        }

        private async Task PersistCache()
        {
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(
                this.cacheFilePath,
                CreationCollisionOption.ReplaceExisting);

            Windows.Storage.FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(cache));
        }
    }
}

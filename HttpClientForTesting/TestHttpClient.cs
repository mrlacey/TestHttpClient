using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

namespace HttpClientForTesting
{
    /// Use cases
    /// =========
    /// - record all for later playback
    /// - playback and error if not in cache
    /// - playback and get live if not in cache
    /// - playback multiple responses for the same request
    /// - playback always the same response for the same repeated request
    /// 
    /// - Twitter app : capture list of tweets so debug issue with something being displayed
    /// - Twitter app : capture list of tweets that includes all tweet types - for checking design support
    /// 
    /// - 

    public class TestHttpClient
    {
        private readonly Dictionary<Uri, string> cache = new Dictionary<Uri, string>();
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
                if (!string.IsNullOrWhiteSpace(settings.LoadCacheFromAppxFile))
                {
                    if (!string.IsNullOrWhiteSpace(settings.LoadCacheFrom))
                    {
                        throw new ArgumentException("Cannot specify both LoadCacheFrom and LoadCacheFromAppxFile");
                    }

                    try
                    {
                        var fldr = Windows.ApplicationModel.Package.Current.InstalledLocation;
                        var file = fldr.GetFileAsync(settings.LoadCacheFromAppxFile).GetResults();
                        var contents = FileIO.ReadTextAsync(file).GetResults();

                        cache = JsonConvert.DeserializeObject<Dictionary<Uri, string>>(contents);

                        Debug.WriteLine("Loaded cache file: {0}", settings.LoadCacheFromAppxFile);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Failed to load cached file '{0}'. Error: {1}", settings.LoadCacheFromAppxFile, exc.Message);
                    }
                }

                if (!string.IsNullOrWhiteSpace(settings.LoadCacheFrom))
                {
                    try
                    {
                        var fldr = Windows.Storage.ApplicationData.Current.LocalFolder;
                        var file = fldr.GetFileAsync(settings.LoadCacheFrom).GetResults();
                        var contents = FileIO.ReadTextAsync(file).GetResults();

                        cache = JsonConvert.DeserializeObject<Dictionary<Uri, string>>(contents);

                        Debug.WriteLine("Loaded cache file: {0}", settings.LoadCacheFromAppxFile);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Failed to load cached file '{0}'. Error: {1}", settings.LoadCacheFromAppxFile, exc.Message);
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

            // TODO: Have a testable solution for abstracting the data access
            //Debug.WriteLine("Cache will be saved to '{0}\\{1}'", ApplicationData.Current.LocalFolder.Path, this.cacheFilePath);
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            return await GetStringAsync(new Uri(requestUri));
        }

        public async Task<string> GetStringAsync(Uri requestUri)
        {
            if (this._settings.ShouldFail(requestUri))
            {
                throw new WebException();
            }

            string cachedValue;

            if (TryGetFromCache(requestUri, out cachedValue))
            {
                return cachedValue;
            }

            var client = _handler != null ? new HttpClient(_handler, _disposeHandler)
                                          : new HttpClient();

            var resp = await client.GetStringAsync(requestUri);

            cache.Add(requestUri, resp);

            await this.PersistCache();

            return resp;
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return await PostAsync(new Uri(requestUri), content);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        // TODO: Add tests for this: once have a way to set the cache values to use when testing
        internal bool TryGetFromCache(Uri requestUri, out string cachedValue)
        {
            if (!_settings.IgnoreQueryStringParameters.Any())
            {
                if (cache.ContainsKey(requestUri))
                {
                    cachedValue = cache[requestUri];
                    return true;
                }
            }
            else
            {
                var requestWithoutIgnoredParams = RemoveIgnoredParams(requestUri);

                foreach (var item in cache)
                {
                    if (RemoveIgnoredParams(item.Key) == requestWithoutIgnoredParams)
                    {
                        cachedValue = item.Value;
                        return true;
                    }
                }
            }

            cachedValue = null;
            return false;
        }

        internal string RemoveIgnoredParams(Uri requestUri)
        {
            var qryParams = requestUri.Query.Replace("?", "").Split('&');

            var filteredParams = new List<string>();

            foreach (var param in qryParams)
            {
                if (!_settings.IgnoreQueryStringParameters.Contains(param.Split('=')[0]))
                {
                    filteredParams.Add(param);
                }
            }

            return string.Concat(requestUri.AbsoluteUri.Replace(requestUri.Query, ""), "?", string.Join("&", filteredParams.ToArray()));
        }

        private async Task PersistCache()
        {
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(
                this.cacheFilePath,
                CreationCollisionOption.ReplaceExisting);

            await Windows.Storage.FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(cache));
        }
    }
}

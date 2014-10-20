namespace HttpClientForTesting
{
    public class TestHttpClientSettings
    {
        /// <summary>
        /// Gets or sets the file to load cache data from.
        /// This should be the relative path to a file included in the APPX package.
        /// If not specified then the cache will be empty by default.
        /// </summary>
        public string LoadCacheFrom { get; set; }

        /// <summary>
        /// Gets or sets the name of the file to save the cache to.
        /// This should be a file name to use when the cache is saved to local storage.
        /// If not specified then a time stamp will be used.
        /// </summary>
        public string SaveCacheTo { get; set; }
    }
}
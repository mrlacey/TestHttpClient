
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HttpClientForTesting
{
    public class TestHttpClientSettings
    {
        /// <summary>
        /// Gets or sets the file to load cache data from.
        /// This should be the relative path to a file included in the APPX package.
        /// It should have the 'Build Action' of "Content"
        /// If not specified then the cache will be empty by default.
        /// Cannot set this and LoadCacheFrom
        /// </summary>
        public string LoadCacheFromAppxFile { get; set; }

        /// <summary>
        /// Gets or sets the name of the file to load cache data from.
        /// If not specified then the cache will be empty by default.
        /// Set this as the same value as 'SaveCacheTo' to enable recording and playback of the same data
        /// Cannot set this and LoadCacheFromAppxFile
        /// </summary>
        public string LoadCacheFrom { get; set; }

        /// <summary>
        /// Gets or sets the name of the file to save the cache to.
        /// This should be a file name to use when the cache is saved to local storage.
        /// If not specified then a time stamp will be used.
        /// </summary>
        public string SaveCacheTo { get; set; }

        /// <summary>
        /// String or strings that if are contained in the URI will result
        /// in a WebException being thrown when requested.
        /// </summary>
        public StringOrStringArray FailRequestsContaining { get; set; }

        /// <summary>
        /// String or strings containing a Regex that if matches the URI will result
        /// in a WebException being thrown when requested.
        /// </summary>
        public StringOrStringArray FailRequestsMatchingRegex { get; set; }

        /// <summary>
        /// String or strings containing query string parameters that should not be 
        /// included as part of any comparison of Uris
        /// </summary>
        public StringOrStringArray IgnoreQueryStringParameters { get; set; }

        /// <summary>
        /// Helper function to check if the Uri is one that matches those
        /// that have been marked as automatically failing
        /// </summary>
        /// <param name="requestUri">The Uri to check</param>
        /// <returns>True if the Uri is one that should fail, otherwise false and should be attempted</returns>
        internal bool ShouldFail(Uri requestUri)
        {
            if (FailRequestsContaining.Any(item => requestUri.AbsoluteUri.Contains(item)))
            {
                return true;
            }

            return FailRequestsMatchingRegex.Any(item => new Regex(item).IsMatch(requestUri.AbsoluteUri));
        }

        public class StringOrStringArray
        {
            // CS6 - remove private setter
            public string[] Values { get; private set; }

            // CS6
            //public string this[int id] => Values[id];
            public string this[int id]
            {
                get { return this.Values[id]; }
            }

            private StringOrStringArray(string value)
                : this(new[] { value })
            {
            }

            private StringOrStringArray(string[] values)
            {
                Values = values;
            }

            public static implicit operator StringOrStringArray(string value)
            {
                return new StringOrStringArray(value);
            }

            public static implicit operator StringOrStringArray(string[] values)
            {
                return new StringOrStringArray(values);
            }

            internal bool Contains(string toLookFor)
            {
                return Values.Contains(toLookFor);
            }

            internal bool Any()
            {
                return Values.Length > 0;
            }

            public bool Any(Func<string, bool> func)
            {
                return Values.Any(func);
            }
        }
    }
}
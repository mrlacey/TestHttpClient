using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpClientForTesting
{
    // TODO: move into separate project when VS2015 supports it
    [TestClass]
    public class TestHttpClientSettingsTests
    {
        [TestMethod]
        public void OnlyQueryParameterIsIgnored_IfInIgnoreList()
        {
            var settings = new TestHttpClientSettings
            {
                IgnoreQueryStringParameters = "id"
            };

            var sut = new HttpClientForTesting.TestHttpClient(settings);

            var result = sut.RemoveIgnoredParams(new Uri("http://example.com/?id=123"));

            Assert.AreEqual(result, "http://example.com/?");
        }

        [TestMethod]
        public void MultipleQueryParameterAreAllIgnored_IfAllInIgnoreList()
        {
            var settings = new TestHttpClientSettings
            {
                IgnoreQueryStringParameters = new[] { "id", "ref" }
            };

            var sut = new HttpClientForTesting.TestHttpClient(settings);

            var result = sut.RemoveIgnoredParams(new Uri("http://example.com/?id=123&ref=abc"));

            Assert.AreEqual(result, "http://example.com/?");
        }

        [TestMethod]
        public void OnlyQueryParameterIsIgnored_IfOneOfManyInIgnoreList()
        {
            var settings = new TestHttpClientSettings
            {
                IgnoreQueryStringParameters = new[] { "id", "ref" }
            };

            var sut = new HttpClientForTesting.TestHttpClient(settings);

            var result = sut.RemoveIgnoredParams(new Uri("http://example.com/?ref=abc"));

            Assert.AreEqual(result, "http://example.com/?");
        }

        [TestMethod]
        public void QueryParameterIsIgnoredIfOneOfMany_ButOnlyOneInIgnoreList()
        {
            var settings = new TestHttpClientSettings
            {
                IgnoreQueryStringParameters = new[] { "id" }
            };

            var sut = new HttpClientForTesting.TestHttpClient(settings);

            var result = sut.RemoveIgnoredParams(new Uri("http://example.com/?id=345&ref=abc"));

            Assert.AreEqual(result, "http://example.com/?ref=abc");
        }

        [TestMethod]
        public void NoQueryParametersIgnored_IfNoneInIgnoreList()
        {
            var settings = new TestHttpClientSettings
            {
                IgnoreQueryStringParameters = new[] { "foo" }
            };

            var sut = new HttpClientForTesting.TestHttpClient(settings);

            var result = sut.RemoveIgnoredParams(new Uri("http://example.com/?id=345&ref=abc"));

            Assert.AreEqual(result, "http://example.com/?id=345&ref=abc");
        }
    }
}

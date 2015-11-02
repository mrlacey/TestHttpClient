using System;
using HttpClientForTesting;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace TestHttpClient.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var settings = new TestHttpClientSettings
            {
                IgnoreQueryStringParameters = "id"
            };

            var sut = new HttpClientForTesting.TestHttpClient(settings);

            var result = sut.RemoveIgnoredParams(new Uri("http://example.com/?id=123"));

            Assert.AreEqual(result, "http://example.com/?");
        }
    }
}

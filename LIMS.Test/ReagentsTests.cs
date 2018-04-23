using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;  

namespace LIMS.Test
{
    [Ignore("Not working yet")]
    [TestFixture]
    public class ReagentsTests
    {
        private TestServer _server;
        private HttpClient _client;
        private const string baseUri = "http://localhost";

        [SetUp]
        public void Setup()
        {
            var testAssemblyDir = TestContext.CurrentContext.TestDirectory;
            var mainDir = Path.GetFullPath(Path.Combine(testAssemblyDir, "../../../LIMS/App_Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", mainDir);
            _server = TestServer.Create<Startup>();
            _client = _server.HttpClient;
        }

        [Test]
        public async Task Create()
        {
            var result = await _client.PostAsync($"{baseUri}/", JsonContent(new
            {
                Name = "Test Reagent",
                Quantity = 10,
                ExpiryDate = "23/6/2019",
                ManufacturerCode = "ABC123",
            }));

            Assert.IsTrue(result.IsSuccessStatusCode);

            var response = await ReadResponse(result);
            var id = response["ReagentId"];

            Assert.AreEqual(JTokenType.Integer, id.Type);
        }

        private async Task<JToken> ReadResponse(HttpResponseMessage message)
        {
            var text = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JToken>(text);
        }

        private StringContent JsonContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj));
        }
    }
}

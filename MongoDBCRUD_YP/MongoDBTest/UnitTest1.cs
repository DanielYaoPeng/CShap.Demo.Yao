using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace MongoDBTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var s = new { Age = 11, name = "jane88" };

            using (var client = new HttpClient())
            {
                string jsonPostDate = JsonConvert.SerializeObject(s);

                HttpContent httpContent = new StringContent(jsonPostDate);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Add("username", "txt");
                var response = client.PostAsync("http://localhost:54869/api/Home/InsertData?", httpContent).Result;
                //http://localhost:54869/

                //var response = client.GetAsync("http://localhost:54869/api/Home/InsertData?").Result;
                var result = response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}

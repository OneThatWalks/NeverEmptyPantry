using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using Newtonsoft.Json;

namespace NeverEmptyPantry.Api.IntegrationTests.Util
{
    public static class IntegrationHelpers
    {
        /// <summary>
        /// Serializes an object into a stream
        /// </summary>
        /// <param name="value">The object to serialize</param>
        /// <param name="stream">The stream</param>
        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            {
                using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
                {
                    var js = new JsonSerializer();
                    js.Serialize(jtw, value);
                    jtw.Flush();
                }
            }
        }

        /// <summary>
        /// Creates http content from an object
        /// </summary>
        /// <param name="content">The object</param>
        /// <returns></returns>
        public static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        /// <summary>
        /// Deserializes http content to an object
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="content">The HttpContent</param>
        /// <returns>A task result that represents the deserialized object</returns>
        public static async Task<T> DeserializeHttpContentAsync<T>(HttpContent content)
        {
            T obj;
            using (var sr = new StreamReader(await content.ReadAsStreamAsync()))
            {
                var js = new JsonSerializer();
                obj = (T) js.Deserialize(sr, typeof(T));
            }

            return obj;
        }

        /// <summary>
        /// Uses a client to gather the auth token for requests
        /// </summary>
        /// <param name="client">The http client</param>
        /// <returns>A task result that represents the token</returns>
        public static async Task<string> GetAuthorizationTokenAsync(HttpClient client)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/authenticate"))
            {
                request.Content = CreateHttpContent(new LoginModel()
                {
                    Username = "TestUser1",
                    Password = "Str0ngP@ssword"
                });

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();

                    var token = await response.Content.ReadAsAsync<OperationResult<TokenModel>>();

                    return token.Data.Token;
                }
            }
        }
    }
}
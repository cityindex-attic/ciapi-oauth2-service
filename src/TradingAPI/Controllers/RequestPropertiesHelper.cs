using System.Net.Http;
using System.Threading.Tasks;

namespace TradingAPI.Controllers
{
    public static class RequestPropertiesHelper
    {
        public static int GetRequestId(this HttpRequestMessage httpRequestMessage)
        {
            return GetProperty<int>(httpRequestMessage, "RequestId");
        }

        public static string GetUserAddress(this HttpRequestMessage httpRequestMessage)
        {
            return GetProperty<string>(httpRequestMessage, "UserAddress");
        }

        private static T GetProperty<T>(HttpRequestMessage httpRequestMessage, string key)
        {
            if (httpRequestMessage.Properties.ContainsKey(key))
            {
                return (T)(httpRequestMessage.Properties[key]);
            }
            return default(T);
        }

        public static Task<HttpResponseMessage> ToTask(this HttpResponseMessage responseMessage)
        {
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(responseMessage);
            return tcs.Task;
        }
    }
}
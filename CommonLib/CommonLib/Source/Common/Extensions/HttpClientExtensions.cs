using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Utils;
using CommonLib.Source.Models;
using CommonLib.Source.Models.Interfaces;

namespace CommonLib.Source.Common.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<T> PostJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null) where T : class, new()
            => httpClient.SendJTokenAsync<T>(HttpMethod.Post, requestUri, content);
        public static Task<T> GetJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null) where T : class, new()
            => httpClient.SendJTokenAsync<T>(HttpMethod.Get, requestUri, content);
        public static Task<T> PutJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null) where T : class, new()
            => httpClient.SendJTokenAsync<T>(HttpMethod.Put, requestUri, content);
        public static Task<T> DeleteJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null) where T : class, new()
            => httpClient.SendJTokenAsync<T>(HttpMethod.Delete, requestUri, content);
        public static Task<T> PatchJTokenAsync<T>(this HttpClient httpClient, string requestUri, object content = null) where T : class, new()
            => httpClient.SendJTokenAsync<T>(HttpMethod.Patch, requestUri, content);

        private static async Task<T> SendJTokenAsync<T>(this HttpClient httpClient, HttpMethod method, string requestUri, object content) where T : class, new()
        {
            try
            {
                var requestJson = content.JsonSerialize();
                requestUri = !requestUri.StartsWithAny("http://", "https://") && httpClient.BaseAddress != null
                    ? PathUtils.Combine(PathSeparator.FSlash, httpClient.BaseAddress.AbsoluteUri, requestUri) 
                    : requestUri;
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                var response = await httpClient.SendAsync(new HttpRequestMessage(method, requestUri)
                {
                    Content = method == HttpMethod.Get || requestJson == null ? null : new StringContent(requestJson, Encoding.UTF8, "application/json") // to fix HEAD method cannot have a body.
                });

                response.EnsureSuccessStatusCode();
                if (typeof(T) == typeof(IgnoreResponse))
                    return default;

                var stringContent = await response.Content.ReadAsStringAsync();
                return stringContent.JsonDeserialize().To<T>();
            }
            catch (Exception ex)
            {
                if (typeof(T).In(typeof(IApiResponse), typeof(ApiResponse)) || typeof(T).GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    var apiResponse = (IApiResponse) new T();
                    apiResponse.StatusCode = StatusCodeType.Status500InternalServerError;
                    apiResponse.Message = "API threw an Exception";
                    apiResponse.ResponseException = ex;
                    return (T) apiResponse;
                }
                throw;
            }
        }

        private class IgnoreResponse { }
    }
}

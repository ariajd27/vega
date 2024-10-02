using System.Text.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vega.PittAPI
{
    public static class HttpRequester
    {
        private static readonly JsonSerializerOptions options = new() { IncludeFields = true };

        public static async Task<T> MakeHttpRequestAsync<T>(string url)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("vega", "beta"));

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync() ?? throw new Exception($"Server at {url} returned null, which cannot be deserialized.");
            return JsonSerializer.Deserialize<T>(responseBody, options) ?? throw new Exception($"Deserializing response from {url} somehow returned null.");
        }

        public static async Task<TEntry[]> RequestAllAsync<TEntry, TResponse>(string url) where TResponse : IHttpArrayResponse<TEntry>
        {
            TEntry[] output = [];
            var task = MakeHttpRequestAsync<TResponse>(url);
            await task.ContinueWith(x => output = task.Result.GetContents());
            return output;
        }
    }
}
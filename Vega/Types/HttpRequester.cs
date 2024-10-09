using System.Text.Json;

namespace PittAPI
{
    public static class HttpRequester
    {
        private static readonly JsonSerializerOptions options = new() { IncludeFields = true };

        public static async Task<TResponse> MakeHttpRequestAsync<TResponse>(string url)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("vega", "alpha"));

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync() ?? throw new Exception($"Server at {url} returned null, which cannot be deserialized.");

            TResponse output;
            try
            {
                output = JsonSerializer.Deserialize<TResponse>(responseBody, options) ?? throw new Exception($"Deserializing response from {url} somehow returned null.");
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to deserialize response from {url}:\n{e.Message}\nResponse body was:\n{responseBody}");
            }

            return output;
        }

        public static async Task<TEntry[]> RequestAllAsync<TEntry, TResponse>(string url) where TResponse : IHttpArrayResponse<TEntry>
        {
            var response = await MakeHttpRequestAsync<TResponse>(url);
            return response.GetContents();
        }
    }
}
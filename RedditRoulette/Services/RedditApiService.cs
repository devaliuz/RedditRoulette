using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RedditRoulette.Model;

namespace RedditRoulette.Services
{
    public class RedditApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _decryptedAuth;

        public RedditApiService(IOptions<AppConfig> appConfig, EncryptionService encryptionService)
        {
            _httpClient = new HttpClient();
            _decryptedAuth = encryptionService.Decrypt(appConfig.Value.EncryptedAuth);
        }

        public async Task<RedditChildData> GetRandomPost(string subreddit)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "RoulettePost/1.0");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _decryptedAuth);

            string jsonResponse = await _httpClient.GetStringAsync($"https://oauth.reddit.com/{subreddit}/random");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                try
                {
                    var response = JsonSerializer.Deserialize<List<RedditResponse>>(jsonResponse, options);
                    if (response != null && response.Count > 0 &&
                        response[0].Data?.Children != null &&
                        response[0].Data.Children.Count > 0)
                    {
                        return response[0].Data.Children[0].Data;
                    }
                }
                catch (JsonException ex)
                {
                    throw;
                }
            }
            return null;
        }

        public async Task<List<string>> GetSubreddits(string query)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "RoulettePost/1.0");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _decryptedAuth);

            string baseUrl = "https://oauth.reddit.com/api/search_subreddits";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("include_over_18", "true"),
                new KeyValuePair<string, string>("include_unadvertisable", "true"),
                new KeyValuePair<string, string>("query", query)
            });

            try
            {
                var response = await _httpClient.PostAsync(baseUrl, content);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<SubredditSearchResponse>(jsonResponse, options);
                return data?.Subreddits
                    ?.Select(s => s.Name)
                    .Where(name => name.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                    .ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}
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
        private readonly string _auth = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlNIQTI1NjpzS3dsMnlsV0VtMjVmcXhwTU40cWY4MXE2OWFFdWFyMnpLMUdhVGxjdWNZIiwidHlwIjoiSldUIn0.eyJzdWIiOiJ1c2VyIiwiZXhwIjoxNzIxMDQzODgwLjQ1ODMxNCwiaWF0IjoxNzIwOTU3NDgwLjQ1ODMxNCwianRpIjoiMWRNcGVZVWNacm56bG54OVV4R1RvQ01GaGxtS3J3IiwiY2lkIjoiSnRORWQ1RWktLXJvVUFnN1d1ck9TZyIsImxpZCI6InQyX2IyOHl2MDlwcyIsImFpZCI6InQyX2IyOHl2MDlwcyIsImxjYSI6MTY4MzgwNDA5NTAwMCwic2NwIjoiZUp5S1Z0SlNpZ1VFQUFEX193TnpBU2MiLCJmbG8iOjl9.jvJQJF1XpXdwbavcjxpshGZ5mWH9GWbnGyv94gmUBDQdv9nXT4Rp9zU8jX9RdnTtDpl-s-vdLiHN-Dcniv05tsBI6i-dvrLAu-jTgeIlx8aw30C1rTX9i63RhV7Xg9Z22noU9UFNER3tnpQfJJkaYfxSaClqNEf_LaxWqHuxeGS45DpZ3rzqwyRsNz-6LXzIE1YNeF4_Hk4lDWNxSwL-b7hII5HUaOLbEA3GsBv7PhMSvWPKy_GAlQBziJCkfeF5uv5y2hH9Ifc3RYNvQ2BVdDxqR62jkqjCWVRJsegPuqyIPhQbGiekWRX7MqxUawZXj8tT3BqRI1QAViF57xPLuw";
        private readonly string _decryptedAuth;

        public RedditApiService(IOptions<AppConfig> appConfig, EncryptionService encryptionService)
        {
            _httpClient = new HttpClient();
            _decryptedAuth = encryptionService.Decrypt(appConfig.Value.EncryptedAuth);

        }


        public async Task<RedditPost> GetRandomPost(string subreddit)
        {
            
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "RoulettePost/1.0");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _decryptedAuth);


            string jsonResponse = _httpClient.GetStringAsync($"https://oauth.reddit.com/{subreddit}/random").Result;
            string x = jsonResponse;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                var response = JsonSerializer.Deserialize<List<RedditApiResponse>>(jsonResponse, options);

                if (response != null && response.Count > 0 &&
                    response[0].Data?.Children != null &&
                    response[0].Data.Children.Count > 0)
                {
                    var post = response[0].Data.Children[0].Data;

                    return new RedditPost
                    {
                        Url = $"https://www.reddit.com/media?url={post.Url}"
                    };
                }
            }

            return null;
        }

        public async Task<List<string>> GetSubreddits(string query)
        {
            List<string> subreddits = new List<string>();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "RoulettePost/1.0");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _auth);

            string baseUrl = "https://oauth.reddit.com/api/search_subreddits";

            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("include_over_18", "true"),
        new KeyValuePair<string, string>("include_unadvertisable", "true"),
        new KeyValuePair<string, string>("query", query)
    });

            try
            {
                string jsonResponse = await _httpClient.PostAsync(baseUrl, content).Result.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<SubredditSearchResponse>(jsonResponse, options);

                // Stellen Sie sicher, dass Sie die Subreddit-Namen korrekt extrahieren
                return data?.Subreddits
                    ?.Select(s => s.Name)
                    .Where(name => name.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                    .ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetSubreddits: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
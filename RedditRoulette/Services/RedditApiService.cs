using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using RedditRoulette.Model;

namespace RedditRoulette.Services
{
    public class RedditApiService
    {
        private readonly HttpClient _httpClient;

        public RedditApiService()
        {
            _httpClient = new HttpClient();
        }


        public async Task<RedditPost> GetRandomPost(string subreddit)
        {
            string auth = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlNIQTI1NjpzS3dsMnlsV0VtMjVmcXhwTU40cWY4MXE2OWFFdWFyMnpLMUdhVGxjdWNZIiwidHlwIjoiSldUIn0.eyJzdWIiOiJ1c2VyIiwiZXhwIjoxNzIwOTU1NTQxLjQwMzQ3NywiaWF0IjoxNzIwODY5MTQxLjQwMzQ3NywianRpIjoib1RYS0YybnZ4cHJnRHdZMTNHa2VzVW9ycWlXTkR3IiwiY2lkIjoiSnRORWQ1RWktLXJvVUFnN1d1ck9TZyIsImxpZCI6InQyX2IyOHl2MDlwcyIsImFpZCI6InQyX2IyOHl2MDlwcyIsImxjYSI6MTY4MzgwNDA5NTAwMCwic2NwIjoiZUp5S1Z0SlNpZ1VFQUFEX193TnpBU2MiLCJmbG8iOjl9.lQZ-KU0CNJWfw40ehnl9atmX7amtdZ5yiGCetvrr8Js7NmLB2Le37Z4GNx2l5e9h6BakKbpJEv1pnTgOls18SqJkLfok7LqIwjInpDHv8DSHitQuj5iid3gDnO0aPZITcoJPAUx_LiOLjQOLk5OWKlyJPaZd8q6bMrvOhNNyCeWXGO16w2X8Siq0Z1sumgIT3c3lb_NzMBlfQCBeUwy8RV_PTTmz_4kc8PYlU2LnmiXyEhz31Em63R-ICvt5fvwpBDcfBpdyruCJhXrYrBh5kHMgSiAFTJEV_HOrbzjkjP9WgI6_1U7v0yc0YD5-WazqG2_zZvUx51qy0ELjj2eM0g";
            
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "RoulettePost/1.0");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",auth);
            // Empfangen der JSON-Antwort als String
            //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            string jsonResponse = /*await*/ _httpClient.GetStringAsync($"https://oauth.reddit.com/{subreddit}/random"/*, cts.Token*/).Result;
            string x = jsonResponse;
            // Hier können Sie jsonResponse überprüfen oder loggen, falls nötig

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
    }
}
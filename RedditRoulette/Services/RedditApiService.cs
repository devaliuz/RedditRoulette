using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using RedditRoulette.Model;
using RedditRoulette.Services;
using RedditRoulette.ViewModel;


namespace RedditRoulette.Services
{
    public class RedditApiService
    {
        private readonly HttpClient _httpClient;

        public RedditApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");
        }
        //0OpBSn8-VS_6Ms1YQaI5uQ
        //PostRoulette
        public async Task<RedditPost> GetRandomPost(string subreddit)
        {
            var response = await _httpClient.GetFromJsonAsync<List<RedditApiResponse>>($"https://www.reddit.com/r/{subreddit}/random.json");
            var post = response[0].Data.Children[0].Data;

            return new RedditPost
            {
                Title = post.Title,
                Author = post.Author,
                Subreddit = post.Subreddit,
                Url = post.Url,
                Thumbnail = post.Thumbnail,
                Selftext = post.Selftext
            };
        }
    }

    public class RedditApiResponse
    {
        public RedditData Data { get; set; }
    }

    public class RedditData
    {
        public List<RedditChild> Children { get; set; }
    }

    public class RedditChild
    {
        public RedditChildData Data { get; set; }
    }

    public class RedditChildData
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Subreddit { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public string Selftext { get; set; }
    }
}

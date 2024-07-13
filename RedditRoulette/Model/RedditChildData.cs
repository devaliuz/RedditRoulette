using System.Text.Json.Serialization;

namespace RedditRoulette.Model
{
    public class RedditChildData
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("subreddit")]
        public string Subreddit { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonPropertyName("selftext")]
        public string Selftext { get; set; }
    }
}

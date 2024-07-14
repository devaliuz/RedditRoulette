using System.Text.Json.Serialization;

namespace RedditRoulette.Model
{
    public class RedditChild
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("data")]
        public RedditChildData Data { get; set; }
    }
}

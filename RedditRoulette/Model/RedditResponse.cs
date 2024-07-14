using System.Text.Json.Serialization;

namespace RedditRoulette.Model
{
    public class RedditResponse
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("data")]
        public RedditData Data { get; set; }
    }
}

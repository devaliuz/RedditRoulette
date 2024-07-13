using System.Text.Json.Serialization;

namespace RedditRoulette.Model
{
    public class RedditApiResponse
    {
        [JsonPropertyName("data")]
        public RedditData Data { get; set; }
    }
}

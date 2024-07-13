using System.Text.Json.Serialization;
using RedditRoulette.Services;

namespace RedditRoulette.Model
{

    public class RedditChild
    {
        [JsonPropertyName("data")]
        public RedditChildData Data { get; set; }
    }
}

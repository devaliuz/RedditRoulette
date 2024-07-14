using System.Text.Json.Serialization;


namespace RedditRoulette.Model
{

    public class RedditData
    {
        [JsonPropertyName("children")]
        public List<RedditChild> Children { get; set; }
    }
}

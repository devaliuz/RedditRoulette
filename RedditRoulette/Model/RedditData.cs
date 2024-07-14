using System.Text.Json;
using System.Text.Json.Serialization;

namespace RedditRoulette.Model
{
    public class RedditData
    {
        [JsonPropertyName("after")]
        public string After { get; set; }

        [JsonPropertyName("dist")]
        public JsonElement Dist { get; set; }

        [JsonPropertyName("modhash")]
        public string Modhash { get; set; }

        [JsonPropertyName("geo_filter")]
        public string GeoFilter { get; set; }

        [JsonPropertyName("children")]
        public List<RedditChild> Children { get; set; }
    }
}
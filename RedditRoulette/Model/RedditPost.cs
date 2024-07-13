using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditRoulette.Model
{
    public class RedditPost
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Subreddit { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public string Selftext { get; set; }
    }
}

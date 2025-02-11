using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    internal class RedditService
    {
        private static readonly string userAgent = "DiscordBot/1.0 by YourUsername";
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> GetRandomMeme()
        {
            Random rnd = new Random();

            string[] subreddits = { "memes", "dankmemes", "funny", "wholesomememes", "me_irl", "comedyheaven" };
            string randomSubreddit = subreddits[rnd.Next(subreddits.Length)];

            string[] sortingMethods = { "new", "rising", "hot", "top" };
            string randomSort = sortingMethods[rnd.Next(sortingMethods.Length)];

            string timeRange = randomSort == "top" ? (rnd.Next(2) == 0) ? "day" : "week" : "";

            string url = randomSort == "top"
                 ? $"https://www.reddit.com/r/{randomSubreddit}/{randomSort}.json?limit=20&t={timeRange}"
                 : $"https://www.reddit.com/r/{randomSubreddit}/{randomSort}.json?limit=20";

            
                _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);

                try
                {
                    string json = await _httpClient.GetStringAsync(url);
                    var data = JObject.Parse(json);
                    var posts = data["data"]["children"];

                    if (posts != null && posts.HasValues)
                    {
                        var randomPost = posts[rnd.Next(posts.Count())]["data"];
                        string memeTitle = randomPost["title"].ToString();
                        string memeUrl = randomPost["url"].ToString();

                        return $"**{memeTitle}**\n{memeUrl}";
                    }
                    else
                    {
                        return "Could not fetch memes. Try again later.";
                    }
                }
                catch
                {
                    return "Error fetching memes. The API might be down";
                }
            

        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    internal class FactService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string apiUrl = "https://uselessfacts.jsph.pl/api/v2/facts/random?language=en";

        public async Task<string> GetRandomFact()
        {
            try
            {
                string json = await _httpClient.GetStringAsync(apiUrl);
                var data = JObject.Parse(json);
                return $"🧠 **Useless Fact:** {data["text"].ToString()}";
            }
            catch
            {
                return "❌ Error fetching a useless fact. Try again later!";
            }
        }
    }
}

using Discord;
using Discord.WebSocket;
using DiscordBot.Services;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private static string _token = Environment.GetEnvironmentVariable("TOBY_BOT_TOKEN");
        private static readonly ulong _channelId = ulong.TryParse(Environment.GetEnvironmentVariable("DISCORD_CHANNEL_ID"), out ulong id) ? id : 0;
        private readonly RedditService _redditService = new RedditService();
        private readonly FactService _factService = new FactService();

        static async Task Main() => await new Program().RunBotAsync();

        public async Task RunBotAsync()
        {
            if (string.IsNullOrWhiteSpace(_token))
            {
                Console.WriteLine("ERROR: Bot token is missing! Set the environment variable TOBY_BOT_TOKEN.");
                return;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
            });


            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;

            await _client.LoginAsync(Discord.TokenType.Bot, _token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is online!");

            foreach (var guild in _client.Guilds) // Loopa igenom alla servrar boten är i
            {
                Console.WriteLine($"🔹 Server: {guild.Name} ({guild.Id})");

                foreach (var textChannel in guild.TextChannels) // Loopa igenom alla textkanaler
                {
                    Console.WriteLine($"- Kanaler: {textChannel.Name} ({textChannel.Id})");
                }
            }

            
            var channel = _client.GetChannel(_channelId) as IMessageChannel;

            if (channel != null)
            {
                await channel.SendMessageAsync("Hello shaggers! I'm online!");
            }
            else
            {
                Console.WriteLine("WARNING: Could not find the specified channel. Make sure the bot has access to it.");
            }
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            ulong allowedChannelId = _channelId;

            if (message.Channel.Id != allowedChannelId) return;
            if (message.Channel is IDMChannel) return;

            /*----------------------------------------------------------------------------------*/
            if (message.Content.ToLower() == "!ping")
            {
                await message.Channel.SendMessageAsync("Pung!");
            }
            /*----------------------------------------------------------------------------------*/

            string messageContent = message.Content.ToLower();

            /*----------------------------------------------------------------------------------*/
            string[] triggerWords = { "två gånger", "två ggr","2 gånger","2 ggr", "2ggr" };

            if (triggerWords.Any(messageContent.Contains))
            {
                await message.Channel.SendMessageAsync("Det är fan ditt sämsta skämt");
            }
            /*----------------------------------------------------------------------------------*/
            if (message.Content.ToLower().StartsWith("!meme"))
            {
                string meme = await _redditService.GetRandomMeme();
                await message.Channel.SendMessageAsync(meme);
            }
            /*----------------------------------------------------------------------------------*/
            if (message.Content.ToLower().StartsWith("!fact"))
            {
                string fact = await _factService.GetRandomFact();  
                await message.Channel.SendMessageAsync(fact);
            }
        }
    }
}

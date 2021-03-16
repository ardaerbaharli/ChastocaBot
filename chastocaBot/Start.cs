using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace chastocaBot
{
    class Start
    {
        public static string channelName;
        public static string botName = "TwitchBotName";
        public static string botToken = "";
        public static string clientId;
        public static string streamerToken;
        public static string connectionString;
        private static void Main()
        {            
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSecrets.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            botName = configuration.GetSection("TwitchSettings").GetConnectionString("botName");
            botToken = configuration.GetSection("TwitchSettings").GetConnectionString("botToken");
            clientId = configuration.GetSection("TwitchSettings").GetConnectionString("clientId");
            streamerToken = configuration.GetSection("TwitchSettings").GetConnectionString("streamerToken");
            connectionString = configuration.GetSection("DatabaseSettings").GetConnectionString("connectionString");

            ChannelBot bot = new ChannelBot();
            Console.WriteLine("Channel name:");
            channelName = Console.ReadLine();
            bot.Connect();
            Console.ReadLine();
            bot.Disconnect();
        }
    }
}
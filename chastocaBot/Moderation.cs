using System;
using TwitchLib.Client.Extensions;

namespace chastocaBot
{
    class Moderation
    {
        public static void Timeout(string username, int seconds)
        {
            Console.WriteLine("Timing out {0} for {1} seconds.", username, seconds);
            ChannelBot.client.TimeoutUser(Start.channelName, username, TimeSpan.FromSeconds(seconds));
        }
        public static void Ban(string username)
        {
            Console.WriteLine("Banning {0}.", username);
            ChannelBot.client.BanUser(Start.channelName, username);
        }
    }
}
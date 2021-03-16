using System;
using System.Threading.Tasks;
using TwitchLib.Client.Extensions;

namespace chastocaBot
{
    class ChannelEvents
    {
        public static async Task<bool> ChangeGame(string game)
        {
            try
            {
                Console.WriteLine("Changing the game.");
                await ChannelBot.api.V5.Channels.UpdateChannelAsync(ChannelBot.channelId, null, game);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        public static async Task<bool> ChangeTitle(string streamTitle)
        {
            try
            {
                Console.WriteLine("Changing the title.");
                await ChannelBot.api.V5.Channels.UpdateChannelAsync(ChannelBot.channelId, streamTitle);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        public static void Raid(string channelToRaid)
        {
            Console.WriteLine("Raiding" + channelToRaid +".");
            ChannelBot.client.Raid(Start.channelName, channelToRaid);
        }
        public static void SubOnlyOn()
        {           
            Console.WriteLine("Setting sub mode on.");
            ChannelBot.client.SubscribersOnlyOn(Start.channelName);
        }
        public static void SubOnlyOff()
        {
            Console.WriteLine("Setting sub mode off.");
            ChannelBot.client.SubscribersOnlyOff(Start.channelName);
        }
        public static void Host(string userToHost)
        {
            Console.WriteLine("Hosting to" + userToHost + ".");
            ChannelBot.client.Host(Start.channelName, userToHost);
        }
    }
}
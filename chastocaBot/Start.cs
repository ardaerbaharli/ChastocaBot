using System;
using System.Threading;

namespace chastocaBot
{
    class Start
    {
        public static string channelName;
        public static string botName = "chastocaBot";
        public static string botToken = "enter the token of bot";
        public static string clientId = "gp762nuuoqcoxypju8c569th9wz7q5";
        public static string streamerToken = "enter the token of streamer";

        private static void Main(string[] args)
        {
            if (!IsNullOrEmpty(args))
            {
                channelName = args[0];
                Console.WriteLine(channelName);
            }
            else
            {
                Console.WriteLine("Channel name:");
                channelName = Console.ReadLine();
            }
            // add the channel to the database as running
            // if it is already in the database, change status as stopped
            Console.Title = channelName;
            ChannelBot bot = new ChannelBot();

            bot.Connect();
            Thread.Sleep(-1);
            bot.Disconnect();
        }

        private static bool IsNullOrEmpty(string[] myStringArray)
        {
            return myStringArray == null || myStringArray.Length < 1;
        }
    }
}
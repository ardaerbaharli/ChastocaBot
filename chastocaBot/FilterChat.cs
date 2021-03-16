using System.IO;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace chastocaBot
{
    class FilterChat
    {       
        public static bool Filter(string[] fragmentedMessage, ChatMessage chMessage)
        {
            bool isBlacklisted = BlacklistFilter(fragmentedMessage, chMessage);
            bool isLink = LinkFilter(chMessage);
            bool isASCIIArt = ASCIIArtfilter(chMessage);
            return isBlacklisted || isLink || isASCIIArt;
        }
        private static bool ASCIIArtfilter(ChatMessage chMessage)
        {
            bool isASCIIArt = Regex.IsMatch(chMessage.Message, @"([^a-zA-Z0-9]){4,}");
            return isASCIIArt;
        }
        public static bool BlacklistFilter(string[] fragmentedMessage, ChatMessage chMessage)
        {
            bool isBlacklisted = false;

            string filterPath = "E:\\filterWords\\" + Start.channelName + "_filterWords.txt";
            if (!File.Exists(filterPath))
            {
                File.CreateText(filterPath);
            }

            if (new FileInfo(filterPath).Length != 0)
            {
                using StreamReader sw = new StreamReader(filterPath);
                foreach (string message in fragmentedMessage)
                {
                    if (message != null)
                    {
                        if (sw.ReadToEnd().Contains(message))
                        {
                            Moderation.Timeout(chMessage.Username, 60);
                            isBlacklisted = true; // blacklisted word
                        }
                    }
                }
            }
            return isBlacklisted;
        }
        public static bool LinkFilter(ChatMessage chMessage)
        {
            bool isLink = Regex.IsMatch(chMessage.Message,
                @"([\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

            if (isLink)
            {
                Moderation.Timeout(chMessage.Username, 15);
                LogHandler.TxtLogs(chMessage, isLink);
            }
            return isLink;
        }
        public static void AddToFilter(string filteredText)
        {
            string filterPath = "E:\\filterWords\\" + Start.channelName + "_filterWords.txt";
            using StreamWriter sw = File.AppendText(filterPath);
            sw.WriteLine(filteredText);
        }
    }

}
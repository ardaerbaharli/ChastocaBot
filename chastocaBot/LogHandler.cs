using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib.Client.Models;

namespace chastocaBot
{
    class LogHandler
    {
        public static void TxtLogs(ChatMessage chMessage, bool? isLink)
        {
            string path;
            if (isLink == true)
                path = "E:\\chatLogs\\" + Start.channelName + "\\" + DateTime.Today.ToString("dd-MM-yyyy") + "_LINKS_" + ".txt";
            else
                path = "E:\\chatLogs\\" + Start.channelName + "\\" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";
            System.IO.Directory.CreateDirectory("E:\\chatLogs\\" + Start.channelName);
            File.AppendAllText(path, LogMessage(chMessage));
        }

        public static string LogMessage(ChatMessage chMessage)
        {
            List<string> badges = new List<string>();
            foreach (var badge in chMessage.Badges)
            {
                switch (badge.Key.ToString())
                {
                    case "subscriber":
                        badges.Add(chMessage.SubscribedMonthCount.ToString() + "Month(s)-Subscriber");
                        break;
                    case "bits":
                        badges.Add(chMessage.CheerBadge.CheerAmount.ToString() + " Bits");
                        break;
                    case "partner":
                        badges.Add("Verified");
                        break;
                    case "moderator":
                        badges.Add("Mod");
                        break;
                    case "vip":
                        badges.Add("VIP");
                        break;
                    case "broadcaster":
                        badges.Add("Broadcaster");
                        break;
                    case "sub-gifter":
                        badges.Add("Sub-Gifter");
                        break;
                    default:
                        break;
                }

                // todo first 10 badge + bişey daha vardı
            }
            string logMessage = string.Format("{0} >>>> [{1}] [{2}] [{3}] {4}: {5} {6}  ", DateTime.Now,
           badges.Count >= 1 ? badges[0] : "",
           badges.Count >= 2 ? badges[1] : "",
           badges.Count >= 3 ? badges[2] : "",
           chMessage.Username, chMessage.Message, Environment.NewLine);

            return logMessage;
        }
        public static void ConsoleLog(ChatMessage chMessage)
        {
            Console.WriteLine(DateTime.Now + " >>>>>  " + chMessage.Username + ":" + chMessage.Message);
        }
    }
}
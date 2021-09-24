using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib.Client.Models;

namespace chastocaBot
{
    class LogHandler
    {
        public static void Log(ChatMessage msg = null, bool? isLink = null, SentMessage sentMsg = null)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string logDirectory = Path.Combine(appDataPath, @"Chastoca");
            logDirectory = Path.Combine(logDirectory, "chastocaBotTwitch");
            logDirectory = Path.Combine(logDirectory, "logs");
            logDirectory = Path.Combine(logDirectory, Start.channelName);
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string textFileName = string.Format("{0}.txt", DateTime.Today.ToString("dd-MM-yyyy"));

            if (isLink == true)
                textFileName = string.Format("{0}_Links", textFileName);

            logDirectory = Path.Combine(logDirectory, textFileName);
            File.AppendAllText(logDirectory, ToFile(msg, sentMsg));
            ToConsole(msg, sentMsg);
        }

        public static void Log(Log log)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string logDirectory = Path.Combine(appDataPath, @"Chastoca");
            logDirectory = Path.Combine(logDirectory, "chastocaBotTwitch");
            logDirectory = Path.Combine(logDirectory, "logs");
            logDirectory = Path.Combine(logDirectory, Start.channelName);
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string textFileName = string.Format("{0}.txt", DateTime.Today.ToString("dd-MM-yyyy"));

            if (isLink == true)
                textFileName = string.Format("{0}_Links", textFileName);

            logDirectory = Path.Combine(logDirectory, textFileName);
            File.AppendAllText(logDirectory, ToFile(msg, sentMsg));
            ToConsole(msg, sentMsg);
        }
        public static string ToFile(ChatMessage chMessage = null, SentMessage sentMessage = null)
        {
            var currentBadges = chMessage == null ? sentMessage.Badges : chMessage.Badges;
            string username = chMessage == null ? sentMessage.DisplayName : chMessage.DisplayName;
            string message = chMessage == null ? sentMessage.Message : chMessage.Message;


            List<string> badges = new List<string>();
            foreach (var badge in currentBadges)
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
            string logMessage = DateTime.Now + " >>>> ";
            for (int i = 0; i < badges.Count; i++)
            {
                logMessage += string.Format("[{0}]", badges[i]);
            }
            logMessage += " " + username + " : ";
            logMessage += " " + message;
            logMessage += Environment.NewLine;
            // string logMessage = string.Format("{0} >>>> [{1}] [{2}] [{3}] {4}: {5} {6}  ", DateTime.Now,
            //badges.Count >= 1 ? badges[0] : "",
            //badges.Count >= 2 ? badges[1] : "",
            //badges.Count >= 3 ? badges[2] : "",
            //username, message, Environment.NewLine);

            return logMessage;
        }
        public static void ToConsole(ChatMessage chMessage = null, SentMessage sentMessage = null)
        {
            string username = chMessage == null ? sentMessage.DisplayName : chMessage.DisplayName;
            string message = chMessage == null ? sentMessage.Message : chMessage.Message;
            Console.WriteLine(DateTime.Now + " >>>>>  " + username + " : " + message);
        }
        public static void ReportCrash(Exception ex)
        {
            Log crashReport = new Log();
            crashReport.LogName = "CrashReports";
            crashReport.Sender = " ";
            crashReport.Message = string.Format("Exception message: {0}\nStack trace:\n{1}", ex.Message, ex.StackTrace);
            Log(crashReport);
        }
    }
}
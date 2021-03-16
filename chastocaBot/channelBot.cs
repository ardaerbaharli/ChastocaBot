using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Users;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace chastocaBot
{
    public class ChannelBot
    {
        readonly ConnectionCredentials credentials = new ConnectionCredentials(Start.botName, Start.botToken);
        public static TwitchClient client;
        public static TwitchAPI api;
        public static string channelId;
        private bool filterSwitch = false;
        public async void Connect()
        {
            client = new TwitchClient();
            client.Initialize(credentials, Start.channelName);
            client.OnConnected += Client_OnConnected;
            client.OnMessageReceived += Client_OnMessageReceivedAsync;
            client.OnReSubscriber += Client_OnReSubscriber;
            client.Connect();

            api = new TwitchAPI();
            api.Settings.ClientId = Start.clientId;
            api.Settings.AccessToken = Start.botToken;

            Users usr = await api.V5.Users.GetUserByNameAsync(Start.channelName);
            channelId = usr.Matches.First().Id;
        }
        private void Client_OnReSubscriber(object sender, TwitchLib.Client.Events.OnReSubscriberArgs e)
        {
            client.SendMessage(Start.channelName, $"{e.ReSubscriber.DisplayName} celebrating {e.ReSubscriber.Months}. month!");
        }       
        private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Console.WriteLine("Connected");
        }
        public async void Client_OnMessageReceivedAsync(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            string message = e.ChatMessage.Message;
            string[] fragmentedMessage = message.Split(' ', ',', '.', ':', ';', '\t');

            LogHandler.TxtLogs(e.ChatMessage, null);
            LogHandler.ConsoleLog(e.ChatMessage);

            if (filterSwitch == true && (e.ChatMessage.IsModerator == false || e.ChatMessage.IsBroadcaster == false || e.ChatMessage.IsVip == false))
                if (FilterChat.Filter(fragmentedMessage, e.ChatMessage) == true)
                    return;
            string chatCommand = fragmentedMessage.First();
            string displayName = e.ChatMessage.DisplayName;
            // authorized commands
            if (IsAuthorized(e.ChatMessage))
            {
                switch (chatCommand)
                {
                    case "!exit":
                        Disconnect();
                        break;
                    case "!title":
                        await ChangeTitle(displayName, message);
                        break;
                    case "!game":
                        await ChangeGame(displayName, message);
                        break;
                    case "!clip":
                        await CreateClip(e.ChatMessage);
                        break;
                    case "!addcommand":
                        AddCommand(fragmentedMessage, message, displayName);
                        break;
                    case "!changecommand":
                        ChangeCommand(fragmentedMessage, message, displayName);
                        break;
                    case "!deletecommand":
                        DeleteCommand(fragmentedMessage, displayName);
                        break;
                    case "!filter":
                        AddBlacklistWord(fragmentedMessage, message, displayName);
                        break;
                    case "!fon":
                        FilterOn(displayName);
                        break;
                    case "!foff":
                        FilterOff(displayName);
                        break;
                    case "!raid":
                        if (!string.IsNullOrEmpty(fragmentedMessage[1]))
                            ChannelEvents.Raid(fragmentedMessage[1]);
                        break;
                    case "!host":
                        Host(fragmentedMessage, displayName);
                        break;
                    case "!subonly":
                        ChannelEvents.SubOnlyOn();
                        break;
                    case "!subonlyoff":
                        ChannelEvents.SubOnlyOff();
                        break;
                    case "!to":
                        Timeout(fragmentedMessage);
                        break;
                    case "!ban":
                        Ban(fragmentedMessage);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (chatCommand.Equals("!uptime"))
                {
                    await Uptime();
                }
                else if (chatCommand.StartsWith("!"))
                {
                    // check if the message exist and if exist get the answer
                    if (DatabaseHandler.DoesExistInCommands(fragmentedMessage[0]))
                    {
                        string answer = DatabaseHandler.FindCommand(fragmentedMessage[0]);
                        client.SendMessage(Start.channelName, $"{answer} @{e.ChatMessage.DisplayName}");
                    }
                }
            }           
        }
        private void Ban(string[] fragmentedMessage)
        {
            string username = fragmentedMessage[1];
            if (!string.IsNullOrEmpty(username))
                Moderation.Ban(username);
            else
                Console.WriteLine("There is no user in the comamnd.");
        }
        private void Timeout(string[] fragmentedMessage)
        {
            string username = fragmentedMessage[1];
            int seconds = Convert.ToInt32(fragmentedMessage[2]);
            if (!string.IsNullOrEmpty(username) && seconds > 0)
                Moderation.Timeout(username, seconds);
            else
                Console.WriteLine("There is no user or seconds in the comamnd.");
        }
        private void Host(string[] fragmentedMessage, string displayName)
        {
            string channelToHost = fragmentedMessage[1];
            if (!string.IsNullOrEmpty(channelToHost))
            {
                client.SendMessage(Start.channelName, $"Hosting to {channelToHost}! @{displayName}");
                ChannelEvents.Host(channelToHost);
            }
        }
        private void FilterOff(string displayName)
        {
            filterSwitch = false;
            Console.WriteLine("Filtering is off.");
            client.SendMessage(Start.channelName, $"Filtering is off! @{displayName}");
        }
        private void FilterOn(string displayName)
        {
            filterSwitch = true;
            Console.WriteLine("Filtering is on.");
            client.SendMessage(Start.channelName, $"Filtering is on! @{displayName}");
        }
        private void AddBlacklistWord(string[] fragmentedMessage, string message, string displayName)
        {
            string filteredText = message[(fragmentedMessage[0].Length + 1)..];
            if (!string.IsNullOrEmpty(filteredText))
            {
                FilterChat.AddToFilter(filteredText);
                client.SendMessage(Start.channelName, $"Added {filteredText} to blacklist! @{displayName}");
            }
        }
        private void DeleteCommand(string[] fragmentedMessage, string displayName)
        {
            bool isSuccessful;
            string command = fragmentedMessage[1];
            if (!string.IsNullOrEmpty(command))
            {
                isSuccessful = DatabaseHandler.DeleteCommand(command);
                if (isSuccessful)
                {
                    Console.WriteLine("Deleted the command.");
                    client.SendMessage(Start.channelName, $"Deleted {command} command successfully! @{displayName}");
                }
                else
                {
                    Console.WriteLine("Couldn't delete the command.");
                    client.SendMessage(Start.channelName, $"Something went wrong! @{displayName}");
                }
            }
        }
        private void ChangeCommand(string[] fragmentedMessage, string message, string displayName)
        {
            bool isSuccessful;
            string command = fragmentedMessage[1];
            string newCommand = fragmentedMessage[2];
            int startPoint = command.Length + newCommand.Length + 16;
            int commandLength = message.Length - startPoint;
            string answer = message.Substring(startPoint, commandLength).Trim();

            if (answer == "")
            {
                answer = newCommand;
                newCommand = command;
            }
            if (!string.IsNullOrEmpty(command) && !string.IsNullOrEmpty(newCommand) && !string.IsNullOrEmpty(answer))
            {
                isSuccessful = DatabaseHandler.ChangeCommand(command, newCommand, answer);
                if (isSuccessful)
                {
                    Console.WriteLine("Changed the command.");
                    client.SendMessage(Start.channelName, $"Updated successfully! @{displayName}");
                }
                else
                {
                    Console.WriteLine("Couldn't change the command.");
                    client.SendMessage(Start.channelName, $"Something went wrong! @{displayName}");
                }
            }
        }
        private void AddCommand(string[] fragmentedMessage, string message, string displayName)
        {
            bool isSuccessful;
            string command = fragmentedMessage[1];
            int commmandLength = command.Length + 11;
            int textLength = message.Length;
            string answer = message[commmandLength..textLength].Trim();
            if (!string.IsNullOrEmpty(command) && !string.IsNullOrEmpty(answer))
            {
                isSuccessful = DatabaseHandler.AddCommand(command, answer);
                if (isSuccessful)
                {
                    Console.WriteLine("Command added.");
                    client.SendMessage(Start.channelName, $"{command} command added successfully! @{displayName}");
                }
                else
                {
                    Console.WriteLine("Command couldn't added.");
                    client.SendMessage(Start.channelName, $"Something went wrong! @{displayName}");
                }
            }
            else
            {
                Console.WriteLine("Command or answer is empty.");
            }
        }
        private async Task ChangeGame(string displayName, string message)
        {
            string game = "";
            if (message.Length > 6)
                game = message[6..];
            if (!string.IsNullOrEmpty(game))
            {

                bool didGameChange = await ChannelEvents.ChangeGame(game);
                if (didGameChange)
                {
                    Console.WriteLine("Game changed.");
                    client.SendMessage(Start.channelName, $"Game category updated to {game}! @{displayName}");
                }
            }
            else
            {
                Console.WriteLine("There is no game in the command.");
            }
        }
        private async Task ChangeTitle(string displayName, string message)
        {
            string streamTitle = "";
            if (message.Length > 7)
                streamTitle = message[7..];
            if (!string.IsNullOrEmpty(streamTitle))
            {
                bool didTitleChange = await ChannelEvents.ChangeTitle(streamTitle);
                if (didTitleChange)
                {
                    Console.WriteLine("Title changed.");
                    client.SendMessage(Start.channelName, $"Stream title changed to {streamTitle}! @{displayName}");
                }
            }
            else
            {
                Console.WriteLine("There is no title in the command.");
            }
        }
        private bool IsAuthorized(ChatMessage chatMessage)
        {
            return (chatMessage.IsModerator || chatMessage.IsBroadcaster);
        }
        private async Task Uptime()
        {
            Console.WriteLine("Getting the uptime.");
            bool isLive = await api.V5.Streams.BroadcasterOnlineAsync(channelId);

            if (isLive)
            {
                var uptime = await api.V5.Streams.GetUptimeAsync(channelId);
                if (uptime.HasValue)
                {
                    Console.WriteLine("Uptime received.");
                    client.SendMessage(Start.channelName, $"It's been " + uptime.Value.Hours + " hours " + uptime.Value.Minutes + " minutes " + uptime.Value.Seconds + " seconds since stream started!");
                }
                else
                {
                    Console.WriteLine("Couldn't receive the uptime.");
                    client.SendMessage(Start.channelName, "Couldn't receive the uptime, try again.");
                }
            }
            else
            {
                Console.WriteLine("Stream is offline.");
                client.SendMessage(Start.channelName, "Stream is offline.");
            }
        }
        private async Task CreateClip(ChatMessage chatMessage)
        {
            Console.WriteLine("Checking if the stream is live.");
            bool isOnline = await api.V5.Streams.BroadcasterOnlineAsync(channelId);
            if (isOnline)
            {
                try
                {
                    Console.WriteLine("Creating a clip.");
                    var clipResponse = await api.Helix.Clips.CreateClipAsync(channelId).ConfigureAwait(false);  // get clip response
                    string clipId = clipResponse.CreatedClips[0].Id; // get clip id

                    await Task.Delay(5000);
                    var clip = await api.Helix.Clips.GetClipAsync(clipId).ConfigureAwait(false); // get the clip object
                    await Task.Delay(5000);
                    string clipURL = clip.Clips.First().Url; // get clip url
                    client.SendMessage(Start.channelName, $"{clipURL} @{chatMessage.DisplayName}");
                    Console.WriteLine("Successfully created a clip.");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong!");
                    client.SendMessage(Start.channelName, $"Something went wrong! @{chatMessage.DisplayName}");
                    Console.WriteLine(ex);
                }
            }
            else
            {
                client.SendMessage(Start.channelName, $"Stream is offline! @{chatMessage.DisplayName}");
                Console.WriteLine("Stream is offline.");
            }
        }
        internal void Disconnect()
        {
            Console.WriteLine("Disconnected.");
            Environment.Exit(0);
        }
    }
}
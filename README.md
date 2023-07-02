# chastocaBot
Not totally complete but ~~working~~ **not working** Twitch moderation chat bot.

<b>Commands</b>

- !exit >> Exit the bot.
- !title \<title> >> Change the stream title.
- !game \<game> >> Change the stream game.
- !uptime >> Shows the uptime.
- !clip >> Creates a clip for last 30 seconds.
- !addcommand \<command> <answer> >> Adds a command.
- !changecommand \<command> <newCommand> <answer> >> Changes the command.
- !deletecommand \<command> >> Deletes the command.
- !filter \<blacklistWord> >> Adds the word to the blacklist.
- !fon >> Sets the filtering on.
- !foff >> Sets the filtering off.
- !host \<channelName> >> Hosts to the specified channel.
- !raid \<channelName> >> Raids the specified channel.
- !subonly >> Sub-only chat mod on.
- !subonlyoff >> Sub-only chat mod.
- !to \<username> <seconds> >> Timeouts speciifed user for specified seconds. 
- !ban \<username> >> Ban the user.


<b> Works with SQL database. </b>
 
 <b> SETUP </b>
 
 - Edit the "appSecrets.json" file.
    - botName : name of your bot account.
    - botToken : go to website twitchtokengenerator.com and generate custom scope token that scopes everything.
    - clientId : in the same website as botToken, under the Refresh Token.
    - streamerToken : same as the botToken but do it in your channel account.
    - connectionString : string to connect your database. (If you are using local database, it should be something like that: "Data Source=<SERVER_NAME>;Initial Catalog=<DATABASE_NAME>;Integrated Security=True" .)
 - You can change the command texts in channelBot.cs file.
 
 - After you've done everything, start the program and it will ask you a channel name to connect. When you enter, wait till you see "Connected" (3-5 seconds). After you see that, bot is ready to go. (If you don't see "Connected" in about 7-8 seconds, probably something is wrong.)
 
<b> If it is not working, it is because probably one of the those: </b>
 - botName, botToken, clientId or/and streamerToken is not updated,
 - or TwitchLib library has changed,
 - or database is not working.
    

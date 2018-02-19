# StatBot [<img src="https://moonraven.visualstudio.com/_apis/public/build/definitions/5557531f-8a79-4c7b-bde1-75757f001741/4/badge"/>]
A Discord Bot which logs in the HydraIRC complient log format so that mIRCStats will work. Slightly modified parser can be found in the mIRCStats parser folder. 

Download: [![release](http://github-release-version.herokuapp.com/github/MJHeijster/StatBot/release.png)](https://github.com/MJHeijster/StatBot/releases/latest)

# Setup
Compile (or download) and place in a location you want the software in. Configure the application through the StatBot.exe.config file. More information can be found here about the token: https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token

You can either let the bot log to channelname.log or channelid.log. You can figure out what channel belongs to a channelid by opening the log and checking the first line, or by writing \#channelname in Discord. It will show something like <#296923759988703232> where 296923759988703232 is the id.

# Tips
If a channel is renamed and the bot is set to log to channelname.log, you can use a script to move the old and new log files into a folder. If you specify the channel name in the first log (for this example, 1.log) through the following line: 

"*** Now talking in #channelname"

mIRCStats will recognize that name.

Example batch script for combining multiple logs:

> - mkdir C:\Discord\StatBot\277418737798479872\general
> - copy "C:\Discord\StatBot\277418737798479872\general.log" "C:\Discord\StatBot\277418737798479872\general\1.log" /Y
> - copy "C:\Discord\StatBot\277418737798479872\the-toy-factory.log" "C:\Discord\StatBot\277418737798479872\general\2.log" /Y
> - START /WAIT C:\Discord\StatBot\mircstats\mircstats.exe -cfg general.cfg -log "C:\Discord\StatBot\277418737798479872\general\*.log" -html Html\general.html


# Notes
Want to help or need help? Join the discord server https://discord.gg/SFYVQNE or create a pull request.

Font used for the icon: Archicoco